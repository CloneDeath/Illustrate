using System;

namespace Illustrate.Exceptions; 

public class ValidationErrorException : Exception {
	public ValidationErrorException(string message) : base(message) { }
}