using MetaEdit.Conventions;
using System;
using System.Linq;

namespace MetaEdit
{
    public class SuperBackupDecoder : IFileNameDecoder<CallData>
    {
        private readonly IDecodeConvention _convention;

        public SuperBackupDecoder(IDecodeConvention convention)
        {
            _convention = convention ?? new SuperBackupConvention();
        }

        public CallData DecodeFileName(string fileName, params string[] parameters)
        {
            var result = new CallData();
            var components = fileName.Split(_convention.Separators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var callDataType = typeof(CallData);

            if (components[0] != components[1])
            {
                result.ContactName = components[0];
            }
            result.ContactNumber = components[1];

            for (int i = 2; i < components.Length; i++)
            {
                var propertyInfo = callDataType.GetProperty(_convention.Convention[i]);
                if (propertyInfo.PropertyType == typeof(DateTime))
                    propertyInfo.SetValue(result, _convention.GetDateTime(components[i]));
                else if (propertyInfo.PropertyType == typeof(CallType))
                    propertyInfo.SetValue(result, _convention.GetCallType(components[i]));
                else if (propertyInfo.PropertyType == typeof(TimeSpan))
                    propertyInfo.SetValue(result, _convention.GetTimeSpan(components[i]));
            }

            if (parameters == null || parameters.Length == 0)
                return result;

            if (parameters.Length > 0)
                throw new ArgumentException($"{nameof(SuperBackupDecoder)}.{nameof(DecodeFileName)} expected no parameters decoding file: {fileName} but found {parameters.Length}: {string.Join(",", parameters)}");

            result.CallDuration = _convention.GetTimeSpan(parameters.Single());
            return result;
        }
    }
}