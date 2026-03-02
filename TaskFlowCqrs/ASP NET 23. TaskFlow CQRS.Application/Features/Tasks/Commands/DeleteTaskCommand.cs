using ASP_NET_23._TaskFlow_CQRS.Application.Common;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks.Commands;

public record DeleteTaskCommand(int Id) : IRequest<bool>;