﻿using MetaEdit.Conventions;
using System;
using System.Linq;

namespace MetaEdit
{
    public class CallDataDecoder : IFileNameDecoder<CallData>
    {
        private readonly IDecodeConvention _convention;

        public CallDataDecoder(IDecodeConvention convention)
        {
            _convention = convention ?? new TotalRecallConvention();
        }

        public CallData DecodeFileName(string fileName, params string[] paramaters)
        {
            var result = new CallData();
            var components = fileName.Split(_convention.Separators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
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

            if (paramaters == null || paramaters.Length == 0)
                return result;

            if (paramaters.Length > 1)
                throw new ArgumentException();

            result.CallDuration = _convention.GetTimeSpan(paramaters.Single());
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