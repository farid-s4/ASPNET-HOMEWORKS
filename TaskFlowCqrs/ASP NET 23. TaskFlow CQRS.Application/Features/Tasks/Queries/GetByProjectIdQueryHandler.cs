using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks.Queries;

public class GetByProjectIdQueryHandler : IRequestHandler<GetByProjectIdQuery, IEnumerable<TaskItemResponseDto>?>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public GetByProjectIdQueryHandler(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskItemResponseDto>?> Handle(GetByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await _taskItemRepository.GetByProjectIdAsync(request.ProjectId);
        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(taskItems);
    }
}