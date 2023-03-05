using System;

namespace MagnusSdk.Mutator.Parameters
{
    public class ParameterVariant
    {

        public readonly object Value;
        
        public bool BoolValue
        {
            get
            {
                try { return Convert.ToBoolean(Value); }
                catch (Exception e) { /* ignored */ }
                return false;
            }
        }

        public string StringValue
        {
            get
            {
                try { return Convert.ToString(Value); }
                catch (Exception e) { /* ignored */ }
                return string.Empty;
            }
        }

        public double DoubleValue
        {
            get
            {
                try { return Convert.ToDouble(Value); }
                catch (Exception e) { /* ignored */ }
                return 0;
            }
        }

        public long LongValue
        {
            get
            {
                try { return Convert.ToInt64(Value); }
                catch (Exception e) { /* ignored */ }
                return 0;
                
            }
        }

        public ParameterVariant(object value)
        {
            Value = value;
        }
    }
}