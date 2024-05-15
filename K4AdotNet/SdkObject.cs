namespace K4AdotNet
{
    public abstract class SdkObject
    {
        protected SdkObject(bool isOrbbec)
        {
            IsOrbbec = isOrbbec;

            if (IsOrbbec)
                Sdk.Orbbec.CheckEnabled();
            else
                Sdk.Azure.CheckEnabled();
        }

        public bool IsOrbbec { get; }

        public bool IsAzure => !IsOrbbec;
    }
}
