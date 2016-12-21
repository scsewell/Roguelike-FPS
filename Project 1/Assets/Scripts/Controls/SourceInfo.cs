namespace InputController
{
    public class SourceInfo
    {
        private SourceType m_type;
        public SourceType Type
        {
            get { return m_type; }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
        }
        
        public SourceInfo(SourceType type, string name)
        {
            m_type = type;
            m_name = name;
        }
    }
}
