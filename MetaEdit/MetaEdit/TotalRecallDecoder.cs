using MetaEdit.Conventions;
using System;
using System.Linq;

namespace MetaEdit
{
    public class TotalRecallDecoder : IFileNameDecoder<CallData>
    {
        private readonly IDecodeConvention _convention;

        public TotalRecallDecoder(IDecodeConvention convention)
        {
            _convention = convention ?? new TotalRecallConvention();
        }

        public CallData DecodeFileName(string fileName, params string[] parameters)
        {
            var result = new CallData { FileExtension = fileName.Split(".").Last() };
            var name = fileName.Substring(0, fileName.Length - result.FileExtension.Length - 1);
            var components = name.Split(_convention.Separators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var callDataType = typeof(CallData);
            for (int i = 0; i < components.Length; i++)
            {
                var propertyInfo = callDataType.GetProperty(_convention.Convention[i]);
                if (propertyInfo.PropertyType == typeof(DateTime))
                    propertyInfo.SetValue(result, _convention.GetDateTime(components[i]));
                else if (propertyInfo.PropertyType == typeof(CallType))
                    propertyInfo.SetValue(result, _convention.GetCallType(components[i]));
                else if (!IsUnknownString(components[i], i))
                    propertyInfo.SetValue(result, components[i]);
            }

            if (parameters == null || parameters.Length == 0)
                return result;

            if (parameters.Length > 1)
                throw new ArgumentException($"{nameof(TotalRecallDecoder)}.{nameof(DecodeFileName)} expected 1 parameter decoding file: {fileName} but found {parameters.Length}: {string.Join(",", parameters)}");

            result.CallDuration = _convention.GetTimeSpan(parameters.Single());
            return result;
        }

        private bool IsUnknownString(string value, int conventionIndex)
        {
            if (conventionIndex == 2 && value == "Unknown.Contact")
                return true;

            if (conventionIndex == 3 && value == "PrivateNumber")
                return true;

            return false;
        }
    }
}