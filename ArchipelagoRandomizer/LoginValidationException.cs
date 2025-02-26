using System;

namespace DDoor.ArchipelagoRandomizer;

internal class LoginValidationException(string message) : Exception(message)
{
}