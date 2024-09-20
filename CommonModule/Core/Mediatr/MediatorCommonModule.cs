using System.Reflection;
using Autofac;
using CommonModule.Core.Mediatr.Auth.Requests;
using MediatR;

namespace CommonModule.Core.Mediatr;

public class MediatorCommonModule: Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(typeof(GetCurrentUserIdRequest).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
    }
}