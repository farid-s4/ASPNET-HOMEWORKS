namespace ASP_16._TaskFlow_Ownership.Mappings;

using ASP_16._TaskFlow_Ownership.DTOs;
using ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;
using ASP_16._TaskFlow_Ownership.Models;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Project
        CreateMap<Project, ProjectResponseDto>()
            .ForMember(
                        dest=>dest.TaskCount, 
                        opt=>opt.MapFrom(src=> src.Tasks.Count));

        CreateMap<CreateProjectDto, Project>()
            .ForMember(dest=>dest.Id, opt=> opt.Ignore())
            .ForMember(dest=> dest.CreatedAt, opt=> opt.MapFrom(src=> DateTime.UtcNow))
            .ForMember(dest=> dest.UpdatedAt, opt=> opt.Ignore())
            .ForMember(dest=> dest.Tasks, opt=> opt.Ignore());

        CreateMap<UpdateProjectDto, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        // TaskItem
        CreateMap<TaskItem, TaskItemResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name));


        CreateMap<CreateTaskItemDto, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt=> opt.MapFrom(src=> Models.TaskStatus.ToDo))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore());

        CreateMap<UpdateTaskItemDto, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>src.Status))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src =>src.Priority))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectId, opt => opt.Ignore());

    }
}
