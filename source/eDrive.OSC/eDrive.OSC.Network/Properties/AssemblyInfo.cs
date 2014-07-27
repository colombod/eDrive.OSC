using System.Reflection;
using System.Runtime.CompilerServices;
using eDrive.Osc.Serialisation;
using eDrive.Osc.Serialisation.Json;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.

[assembly: AssemblyTitle ("eDrive.Osc.Network")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("Diego Colombo")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyCopyright ("Diego Colombo")]
[assembly: AssemblyTrademark ("Diego Colombo")]
[assembly: AssemblyCulture ("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion ("1.0.*")]

// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
[assembly: ContainsOscSerialisers]
[assembly: ContainsOscJsonSerialisers]
