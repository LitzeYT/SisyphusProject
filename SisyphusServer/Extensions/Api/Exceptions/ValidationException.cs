﻿using FluentValidation.Results;

namespace SisyphusServer.Extensions.Api.Exceptions {
    public class ValidationException : Exception {
        public IDictionary<string, string[]> Errors { get; set; }

        public ValidationException() : base("One or more validation failures have occurred.") {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this() {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(group => group.Key, group => group.ToArray());
        }
    }
}