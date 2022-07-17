using autodoc.tasks.domain.Dto.Task;
using autodoc.tasks.domain.Entities;
using AutoMapper;

namespace autodoc.tasks.application.Mappings;

public sealed class TaskMappingProfile : Profile
{
	public TaskMappingProfile()
	{
		CreateMap<TaskEntity, TaskDto>();
	}
}
