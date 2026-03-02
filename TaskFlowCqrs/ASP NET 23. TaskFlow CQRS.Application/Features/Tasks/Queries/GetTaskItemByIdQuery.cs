using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks.Queries;

public record GetTaskItemByIdQuery(int Id) : IRequest<TaskItemResponseDto?>;
