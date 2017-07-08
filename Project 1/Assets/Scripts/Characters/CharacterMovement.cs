using UnityEngine;
using System;
using System.Collections;
using Framework.Interpolation;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    public event Action OnLand;

    [Serializable]
    public class CharacterMotorMovement
    {
        public float maxForwardCrouchSpeed = 10.0f;
        public float maxForwardWalkSpeed = 10.0f;
        public float maxForwardRunSpeed = 10.0f;
        public float maxSidewaysSpeed = 10.0f;
        public float maxBackwardsSpeed = 10.0f;
        public float maxBurdenedSpeed = 4.0f;
        public float heightChangeRate = 6.0f;
        public float crouchHeight = 1.5f;
        public float standHeight = 2.0f;
        
        public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
        
        public float maxGroundAcceleration = 30.0f;
        public float maxAirAcceleration = 20.0f;
        
        public float gravity = 9.81f;
        public float maxFallSpeed = 20.0f;
    }
    
    [Serializable]
    public class CharacterMotorJumping
    {
        public bool enabled = true;
        public float baseHeight = 1.0f;
        public float extraHeight = 4.1f;
        public float perpAmount = 0.0f;
        public float steepPerpAmount = 0.5f;

        [NonSerialized]
        public bool holdingJumpButton = false;
        [NonSerialized]
        public float lastStartTime = 0;
        [NonSerialized]
        public float lastButtonDownTime = -100;
        [NonSerialized]
        public Vector3 jumpDir = Vector3.up;
    }

    public enum MovementTransferOnJump
    {
        None, // The jump is not affected by velocity of floor at all.
        InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
        PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
        PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
    }

    [Serializable]
    public class CharacterMotorMovingPlatform
    {
        public bool enabled = true;
        public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;

        [NonSerialized]
        public Transform hitPlatform;
        [NonSerialized]
        public Transform activePlatform;
        [NonSerialized]
        public Vector3 activeLocalPoint;
        [NonSerialized]
        public Vector3 activeGlobalPoint;
        [NonSerialized]
        public Quaternion activeLocalRotation;
        [NonSerialized]
        public Quaternion activeGlobalRotation;
        [NonSerialized]
        public Matrix4x4 lastMatrix;
        [NonSerialized]
        public Vector3 platformVelocity;
        [NonSerialized]
        public bool newPlatform;
    }

    [Serializable]
    public class CharacterMotorSliding
    {
        public bool enabled = true;
        public float slidingSpeed = 15.0f;
        public float sidewaysControl = 1.0f;
        public float speedControl = 0.4f;
    }

    [SerializeField]
    private CharacterMotorMovement m_movement = new CharacterMotorMovement();
    [SerializeField]
    private CharacterMotorJumping m_jumping = new CharacterMotorJumping();
    [SerializeField]
    private CharacterMotorMovingPlatform m_movingPlatform = new CharacterMotorMovingPlatform();
    [SerializeField]
    private CharacterMotorSliding m_sliding = new CharacterMotorSliding();

    private CharacterController m_controller;
    public CharacterController Controller { get { return m_controller; } }

    private CollisionFlags m_collisionFlags;
    private Vector3 m_groundNormal = Vector3.zero;
    private Vector3 m_lastGroundNormal = Vector3.zero;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_frameVelocity = Vector3.zero;
    private Vector3 m_hitPoint = Vector3.zero;
    private Vector3 m_lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

    private Vector3 m_actualVelocity;
    public Vector3 Velocity { get { return m_actualVelocity; } }

    private bool m_isRunning = false;
    public bool IsRunning { get { return m_isRunning; } }

    private bool m_isGrounded = false;
    public bool IsGrounded { get { return m_isGrounded; } }

    private bool m_isJumping = false;
    public bool IsJumping { get { return m_isJumping; } }

    private bool m_crouching = false;
    public bool IsCrouching { get { return m_crouching; } }

    public Vector3 ColliderTop
    {
        get { return transform.TransformPoint(m_controller.center + ((m_controller.height / 2) * Vector3.up)); }
    }

    public Vector3 ColliderBottom
    {
       get { return transform.TransformPoint(m_controller.center + ((m_controller.height / 2) * Vector3.down)); }
    }

    private void Awake()
    {
        m_controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        float startHeight = m_controller.height;
        m_controller.height = m_movement.standHeight;
        transform.Translate(0, (m_controller.height - startHeight) / 2, 0);

        InterpolatedFloat height = new InterpolatedFloat(() => (m_controller.height), val => { m_controller.height = val; });
        gameObject.AddComponent<FloatInterpolator>().Initialize(height);

        gameObject.AddComponent<TransformInterpolator>().ForgetPreviousValues();
    }

    public void UpdateMovement(MoveInputs input)
    {
        if (!m_controller.enabled)
        {
            return;
        }

        if (m_movingPlatform.enabled)
        {
            if (m_movingPlatform.activePlatform != null)
            {
                if (!m_movingPlatform.newPlatform)
                {
                    m_movingPlatform.platformVelocity = (
                        m_movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(m_movingPlatform.activeLocalPoint)
                        - m_movingPlatform.lastMatrix.MultiplyPoint3x4(m_movingPlatform.activeLocalPoint)
                    ) / Time.deltaTime;
                }
                m_movingPlatform.lastMatrix = m_movingPlatform.activePlatform.localToWorldMatrix;
                m_movingPlatform.newPlatform = false;
            }
            else
            {
                m_movingPlatform.platformVelocity = Vector3.zero;
            }
        }

        Vector3 velocity = m_velocity;

        velocity = ApplyInputVelocityChange(input, velocity);
        velocity = ApplyGravityAndJumping(input, velocity);

        m_crouching = input.Crouch && !input.Run;

        float lastHeight = m_controller.height;
        m_controller.height = Mathf.Lerp(m_controller.height, m_crouching ? m_movement.crouchHeight : m_movement.standHeight, Time.deltaTime * m_movement.heightChangeRate);
        if (!m_crouching)
        {
            transform.Translate(0, (m_controller.height - lastHeight) / 4, 0);
        }

        // Moving platform support
        Vector3 moveDistance = Vector3.zero;
        if (MoveWithPlatform())
        {
            Vector3 newGlobalPoint = m_movingPlatform.activePlatform.TransformPoint(m_movingPlatform.activeLocalPoint);
            moveDistance = (newGlobalPoint - m_movingPlatform.activeGlobalPoint);
            if (moveDistance != Vector3.zero)
            {
                m_controller.Move(moveDistance);
            }

            // Support moving platform rotation as well:
            Quaternion newGlobalRotation = m_movingPlatform.activePlatform.rotation * m_movingPlatform.activeLocalRotation;
            Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(m_movingPlatform.activeGlobalRotation);

            float yRotation = rotationDiff.eulerAngles.y;
            if (yRotation != 0)
            {
                // Prevent rotation of the local up vector
                transform.Rotate(0, yRotation, 0);
            }
        }

        // Save lastPosition for velocity calculation.
        Vector3 lastPosition = transform.position;

        // We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
        Vector3 currentMovementOffset = velocity * Time.deltaTime;

        // Find out how much we need to push towards the ground to avoid loosing grouning
        // when walking down a step or over a sharp change in slope.
        float pushDownOffset = Mathf.Max(m_controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
        if (m_isGrounded)
        {
            currentMovementOffset -= pushDownOffset * Vector3.up;
        }

        // Reset variables that will be set by collision function
        m_movingPlatform.hitPlatform = null;
        m_groundNormal = Vector3.zero;

        // Move our character!
        m_collisionFlags = m_controller.Move(currentMovementOffset);

        m_lastHitPoint = m_hitPoint;
        m_lastGroundNormal = m_groundNormal;

        if (m_movingPlatform.enabled && m_movingPlatform.activePlatform != m_movingPlatform.hitPlatform)
        {
            if (m_movingPlatform.hitPlatform != null)
            {
                m_movingPlatform.activePlatform = m_movingPlatform.hitPlatform;
                m_movingPlatform.lastMatrix = m_movingPlatform.hitPlatform.localToWorldMatrix;
                m_movingPlatform.newPlatform = true;
            }
        }

        // Calculate the velocity based on the current and previous position.  
        // This means our velocity will only be the amount the character actually moved as a result of collisions.
        Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
        m_actualVelocity = (transform.position - lastPosition) / Time.deltaTime;
        m_velocity = m_actualVelocity;
        Vector3 newHVelocity = new Vector3(m_velocity.x, 0, m_velocity.z);

        // The CharacterController can be moved in unwanted directions when colliding with things.
        // We want to prevent this from influencing the recorded velocity.
        if (oldHVelocity == Vector3.zero)
        {
            m_velocity = new Vector3(0, m_velocity.y, 0);
        }
        else
        {
            float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
            m_velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + m_velocity.y * Vector3.up;
        }

        if (m_velocity.y < velocity.y - 0.001f)
        {
            if (m_velocity.y < 0)
            {
                // Something is forcing the CharacterController down faster than it should.
                // Ignore this
                m_velocity.y = velocity.y;
            }
            else
            {
                // The upwards movement of the CharacterController has been blocked.
                // This is treated like a ceiling collision - stop further jumping here.
                m_jumping.holdingJumpButton = false;
            }
        }

        // We were grounded but just loosed grounding
        if (m_isGrounded && !IsGroundedTest())
        {
            m_isGrounded = false;

            // Apply inertia from platform
            if (m_movingPlatform.enabled &&
                (m_movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                m_movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
            )
            {
                m_frameVelocity = m_movingPlatform.platformVelocity;
                m_velocity += m_movingPlatform.platformVelocity;
            }
            
            // We pushed the character down to ensure it would stay on the ground ifthere was any.
            // But there wasn't so now we cancel the downwards offset to make the fall smoother.
            transform.position += pushDownOffset * Vector3.up;
        }
        // We were not grounded but just landed on something
        else if (!m_isGrounded && IsGroundedTest())
        {
            m_isGrounded = true;
            m_isJumping = false;
            SubtractNewPlatformVelocity();
            if (OnLand != null)
            {
                OnLand();
            }
        }

        // Moving platforms support
        if (MoveWithPlatform())
        {
            // Use the center of the lower half sphere of the capsule as reference point.
            // This works best when the character is standing on moving tilting platforms. 
            m_movingPlatform.activeGlobalPoint = transform.position + Vector3.up * (m_controller.center.y - m_controller.height * 0.5f + m_controller.radius);
            m_movingPlatform.activeLocalPoint = m_movingPlatform.activePlatform.InverseTransformPoint(m_movingPlatform.activeGlobalPoint);

            // Support moving platform rotation as well:
            m_movingPlatform.activeGlobalRotation = transform.rotation;
            m_movingPlatform.activeLocalRotation = Quaternion.Inverse(m_movingPlatform.activePlatform.rotation) * m_movingPlatform.activeGlobalRotation;
        }
    }

    private Vector3 ApplyInputVelocityChange(MoveInputs input, Vector3 velocity)
    {
        // Find desired velocity
        Vector3 desiredVelocity;
        if (m_isGrounded && TooSteep())
        {
            // The direction we're sliding in
            desiredVelocity = new Vector3(m_groundNormal.x, 0, m_groundNormal.z).normalized;
            // Find the input movement direction projected onto the sliding direction
            Vector3 projectedMoveDir = Vector3.Project(input.MoveDirection, desiredVelocity);
            // Add the sliding direction, the spped control, and the sideways control vectors
            desiredVelocity = desiredVelocity + projectedMoveDir * m_sliding.speedControl + (input.MoveDirection - projectedMoveDir) * m_sliding.sidewaysControl;
            // Multiply with the sliding speed
            desiredVelocity *= m_sliding.slidingSpeed;
        }
        else
        {
            desiredVelocity = GetDesiredHorizontalVelocity(input);
        }

        if (m_movingPlatform.enabled && m_movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
        {
            desiredVelocity += m_frameVelocity;
            desiredVelocity.y = 0;
        }

        if (m_isGrounded)
        {
            desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, m_groundNormal);
        }
        else
        {
            velocity.y = 0;
        }

        float maxVelocityChange = GetMaxAcceleration(m_isGrounded) * Time.deltaTime;
        velocity += Vector3.ClampMagnitude(desiredVelocity - velocity, maxVelocityChange);

        if (m_isGrounded)
        {
            // When going uphill, the CharacterController will automatically move up by the needed amount.
            // Not moving it upwards manually prevent risk of lifting off from the ground.
            // When going downhill, DO move down manually, as gravity is not enough on steep hills.
            velocity.y = Mathf.Min(velocity.y, 0);
        }

        return velocity;
    }

    private Vector3 ApplyGravityAndJumping(MoveInputs input, Vector3 velocity)
    {
        if (!input.Jump)
        {
            m_jumping.holdingJumpButton = false;
            m_jumping.lastButtonDownTime = -100;
        }

        if (input.Jump && m_jumping.lastButtonDownTime < 0)
        {
            m_jumping.lastButtonDownTime = Time.time;
        }

        if (m_isGrounded)
        {
            velocity.y = Mathf.Min(0, velocity.y) - m_movement.gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = m_velocity.y - m_movement.gravity * Time.deltaTime;

            // When jumping up we don't apply gravity for some time when the user is holding the jump button.
            // This gives more control over jump height by pressing the button longer.
            if (m_isJumping && m_jumping.holdingJumpButton)
            {
                // Calculate the duration that the extra jump force should have effect.
                // ifwe're still less than that duration after the jumping time, apply the force.
                if (Time.time < m_jumping.lastStartTime + m_jumping.extraHeight / CalculateJumpVerticalSpeed(m_jumping.baseHeight))
                {
                    // Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
                    velocity += m_jumping.jumpDir * m_movement.gravity * Time.deltaTime;
                }
            }

            // Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
            velocity.y = Mathf.Max(velocity.y, -m_movement.maxFallSpeed);
        }

        if (m_isGrounded)
        {
            // Jump only if the jump button was pressed down in the last 0.2 seconds.
            // We use this check instead of checking if it's pressed down right now
            // because players will often try to jump in the exact moment when hitting the ground after a jump
            // and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
            // it's confusing and it feels like the game is buggy.
            if (m_jumping.enabled & !input.Burdened && (Time.time - m_jumping.lastButtonDownTime < 0.2))
            {
                m_isGrounded = false;
                m_isJumping = true;
                m_jumping.lastStartTime = Time.time;
                m_jumping.lastButtonDownTime = -100;
                m_jumping.holdingJumpButton = true;

                // Calculate the jumping direction
                m_jumping.jumpDir = Vector3.Slerp(Vector3.up, m_groundNormal, TooSteep() ? m_jumping.steepPerpAmount : m_jumping.perpAmount);

                // Apply the jumping force to the velocity. Cancel any vertical velocity first.
                velocity.y = 0;
                velocity += m_jumping.jumpDir * CalculateJumpVerticalSpeed(m_jumping.baseHeight);

                // Apply inertia from platform
                if (m_movingPlatform.enabled &&
                    (m_movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                    m_movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
                )
                {
                    m_frameVelocity = m_movingPlatform.platformVelocity;
                    velocity += m_movingPlatform.platformVelocity;
                }
            }
            else
            {
                m_jumping.holdingJumpButton = false;
            }
        }

        return velocity;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0 && hit.normal.y > m_groundNormal.y && hit.moveDirection.y < 0)
        {
            if ((hit.point - m_lastHitPoint).sqrMagnitude > 0.001 || m_lastGroundNormal == Vector3.zero)
                m_groundNormal = hit.normal;
            else
                m_groundNormal = m_lastGroundNormal;

            m_movingPlatform.hitPlatform = hit.collider.transform;
            m_hitPoint = hit.point;
            m_frameVelocity = Vector3.zero;
        }
    }

    private IEnumerator SubtractNewPlatformVelocity()
    {
        // When landing, subtract the velocity of the new ground from the character's velocity
        // since movement in ground is relative to the movement of the ground.
        if (m_movingPlatform.enabled &&
          (m_movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
           m_movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
        {
            // if we landed on a new platform, we have to wait for two FixedUpdates
            // before we know the velocity of the platform under the character
            if (m_movingPlatform.newPlatform)
            {
                Transform platform = m_movingPlatform.activePlatform;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                if (m_isGrounded && platform == m_movingPlatform.activePlatform)
                    yield break;
            }
            m_velocity -= m_movingPlatform.platformVelocity;
        }
    }

    private bool MoveWithPlatform()
    {
        return (m_movingPlatform.enabled
            && (m_isGrounded || m_movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
            && m_movingPlatform.activePlatform != null
        );
    }

    private Vector3 GetDesiredHorizontalVelocity(MoveInputs input)
    {
        // Find desired velocity
        Vector3 desiredLocalDirection = transform.InverseTransformDirection(input.MoveDirection);
        float maxSpeed = MaxSpeedInDirection(input, desiredLocalDirection);
        if (m_isGrounded)
        {
            // Modify max speed on slopes based on slope speed multiplier curve
            float movementSlopeAngle = Mathf.Asin(m_velocity.normalized.y) * Mathf.Rad2Deg;
            maxSpeed *= m_movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
        }
        return transform.TransformDirection(desiredLocalDirection * maxSpeed);
    }

    private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
    {
        Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
        return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    }

    private bool IsGroundedTest()
    {
        return (m_groundNormal.y > 0.01);
    }

    private float GetMaxAcceleration(bool grounded)
    {
        return grounded ? m_movement.maxGroundAcceleration : m_movement.maxAirAcceleration;
    }

    private float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt(2 * targetJumpHeight * m_movement.gravity);
    }

    // Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
    // The function returns the length of the resulting vector.
    private float MaxSpeedInDirection(MoveInputs input, Vector3 desiredMovementDirection)
    {
        if (desiredMovementDirection == Vector3.zero)
        {
            return 0;
        }
        else
        {
            float forwardSpeed = m_movement.maxForwardWalkSpeed;
            float backwardsSpeed = m_movement.maxBackwardsSpeed;
            float sidewaysSpeed = m_movement.maxSidewaysSpeed;
            m_isRunning = false;

            if (input.Burdened)
            {
                forwardSpeed = m_movement.maxBurdenedSpeed;
                backwardsSpeed = m_movement.maxBurdenedSpeed;
                sidewaysSpeed = m_movement.maxBurdenedSpeed;
            }
            else if (input.Run)
            {
                m_isRunning = true;
                forwardSpeed = m_movement.maxForwardRunSpeed;
            }
            else if (m_crouching)
            {
                forwardSpeed = m_movement.maxForwardCrouchSpeed;
                backwardsSpeed = m_movement.maxForwardCrouchSpeed;
                sidewaysSpeed = m_movement.maxForwardCrouchSpeed;
            }

            float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? forwardSpeed : backwardsSpeed) / sidewaysSpeed;
            Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
            return new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * sidewaysSpeed;
        }
    }

    private bool TooSteep()
    {
        return (m_groundNormal.y <= Mathf.Cos(m_controller.slopeLimit * Mathf.Deg2Rad));
    }

    public bool IsSliding()
    {
        return (m_isGrounded && m_sliding.enabled && TooSteep());
    }

    public bool IsTouchingCeiling()
    {
        return (m_collisionFlags & CollisionFlags.CollidedAbove) != 0;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && m_controller.enabled)
        {
            Gizmos.color = Color.magenta;
            float offset = (m_controller.height / 2) - m_controller.radius;
            Vector3 top = transform.TransformPoint(m_controller.center + (offset * Vector3.up));
            Vector3 bottom = transform.TransformPoint(m_controller.center + (offset * Vector3.down));

            Gizmos.DrawWireSphere(top, transform.lossyScale.x * m_controller.radius);
            Gizmos.DrawWireSphere(bottom, transform.lossyScale.x * m_controller.radius);
       }
    }
}