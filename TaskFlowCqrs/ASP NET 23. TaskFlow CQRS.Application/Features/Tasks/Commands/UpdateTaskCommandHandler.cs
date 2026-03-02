using ASP_NET_23._TaskFlow_CQRS.Application.Common;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;
using AutoMapper;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks.Commands;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskItemResponseDto>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }

    public async Task<TaskItemResponseDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskItemRepository.GetByIdWithProjectAsync(request.Id);
        if (task is null) return null!;
        _mapper.Map(request.Dto, task);
        await _taskItemRepository.UpdateAsync(task);
        return _mapper.Map<TaskItemResponseDto>(task);
    }
}