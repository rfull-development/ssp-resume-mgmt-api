// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
namespace ResumeManagementApi.Databases.Exceptions
{
    public class DatabaseConflictException : DatabaseException
    {
        public DatabaseConflictException() : base()
        {
        }

        public DatabaseConflictException(string message) : base(message)
        {
        }

        public DatabaseConflictException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
