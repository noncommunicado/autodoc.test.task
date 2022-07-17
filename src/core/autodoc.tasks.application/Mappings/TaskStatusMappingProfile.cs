using autodoc.tasks.domain.Dto.TaskStatus;
using autodoc.tasks.domain.Entities;
using autodoc.tasks.domain.Http.Requests.TaskStatus;
using AutoMapper;

namespace autodoc.tasks.application.Mappings;

public sealed class TaskStatusMappingProfile : Profile
{
	public TaskStatusMappingProfile()
	{
		CreateMap<TaskStatusEntity, TaskStatusDto>();
		CreateMap<CreateTaskStatusRequest, TaskStatusEntity>();
		CreateMap<TaskStatusEntity, TaskStatusExtendedDto>().ReverseMap();
	}
}