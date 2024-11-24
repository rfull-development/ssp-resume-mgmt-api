// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
namespace ResumeManagementApi.Databases.Exceptions
{
    public class DatabaseParameterException : DatabaseException
    {
        public DatabaseParameterException() : base()
        {
        }

        public DatabaseParameterException(string message) : base(message)
        {
        }

        public DatabaseParameterException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
