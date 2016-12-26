using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

	public float enemyCapsuleHeight = 1.65f;
	public float enemyCapsuleRadius = 0.3f;
	public Vector3 enemyCapsuleCenterOffset = new Vector3(0, -0.15f, 0);

	public float unobstructedFudge = 0.1f;

	public float maxAcceleration = 0.01f;
	public float maxSpeed = 0.1f;
	public float frictionLoss = 0.01f;

	//private Vector3 dudeVelocity = Vector2.zero;
	//public when we want to modify it for debugging
	public Vector3 dudeVelocity = Vector2.zero;

	public Vector3 desiredVelocityGUI;
	public Vector3 forcedVelocityGUI;
	
	public void Move (Vector3 minimizingSweepPosition)
	{
		Vector3 desiredVelocity = minimizingSweepPosition - transform.position;
		Vector3 forcedVelocity = dudeVelocity + Vector3.ClampMagnitude(Vector3.Normalize(desiredVelocity - dudeVelocity), maxAcceleration);
		dudeVelocity = Vector3.ClampMagnitude(forcedVelocity, maxSpeed);
		//Vector3 forcedVelocity = dudeVelocity + Vector3.ClampMagnitude(Vector3.Normalize(desiredVelocity - dudeVelocity), maxSpeed * frictionLoss);
		//dudeVelocity = (1 - frictionLoss) * forcedVelocity;
		
		print ("position:"+ transform.position);
		print ("dude:"+ Vector3.Angle(dudeVelocity, Vector3.forward) + ":"+ dudeVelocity.magnitude);

		Vector3 capsuleAxis = new Vector3(0, enemyCapsuleHeight/2, 0);

		bool finishedMoving = false;
		Vector3 velocityToApply = new Vector3(dudeVelocity.x, dudeVelocity.y, dudeVelocity.z);
		Vector3 calculatedPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		int count = 0;

		while (!finishedMoving)
		{
			count++;
			if (count > 20)
			{
				print ("your face");
				break;
			}

			RaycastHit hitInfo;
			bool didHitCollider = Physics.CapsuleCast(calculatedPosition + enemyCapsuleCenterOffset + capsuleAxis , calculatedPosition + enemyCapsuleCenterOffset - capsuleAxis, enemyCapsuleRadius, velocityToApply, out hitInfo, velocityToApply.magnitude);

			print (didHitCollider +": "+ Vector3.Angle(dudeVelocity, Vector3.forward) + ":"+ dudeVelocity.magnitude +": "+ Vector3.Angle(velocityToApply, Vector3.forward) + ":"+ velocityToApply.magnitude);

			if (didHitCollider)
			{
				
				Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal, new Color(0.3f, 0.0f, 0.7f));

				Vector3 unobstructedVelocity = Vector3.ClampMagnitude(velocityToApply, hitInfo.distance);
				//calculatedPosition += unobstructedVelocity;
				//calculatedPosition += Vector3.ClampMagnitude(unobstructedVelocity, unobstructedVelocity.magnitude - unobstructedFudge);
				calculatedPosition += unobstructedVelocity;
				calculatedPosition += Vector3.ClampMagnitude(hitInfo.normal, unobstructedFudge);
				velocityToApply -= unobstructedVelocity;
				velocityToApply -= Vector3.Project(velocityToApply, hitInfo.normal);
				dudeVelocity -= Vector3.Project(dudeVelocity, hitInfo.normal);
			}
			else
			{
				calculatedPosition += velocityToApply;
				finishedMoving = true;
			}
		}

		transform.position = calculatedPosition;
		print ("position after:"+ transform.position);

		desiredVelocityGUI = desiredVelocity;
		forcedVelocityGUI = forcedVelocity;
	}

	public Vector2 vector2FromPolar(float heading, float magnitude)
	{
		return new Vector2(magnitude * Mathf.Cos(heading), magnitude * Mathf.Sin(heading));
	}
	
	public void OnGUI()
	{
		GUILayout.Label(""+desiredVelocityGUI.magnitude);
		GUILayout.Label(""+forcedVelocityGUI.magnitude);
	}
}
