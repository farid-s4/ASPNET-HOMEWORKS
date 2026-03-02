using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;
using AutoMapper;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks.Commands;

public class DeleteTaskCommandHandler :  IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public DeleteTaskCommandHandler(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }
    
    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        TaskItem taskItem = _mapper.Map<TaskItem>(request);
        await _taskItemRepository.RemoveAsync(taskItem);
        return true;
    }
}