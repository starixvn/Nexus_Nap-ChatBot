using System;
using System.Reflection;

namespace Fanex.NCB.Swagger.AuthGate.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}