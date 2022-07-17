using autodoc.tasks.domain.Dto.TaskFile;
using autodoc.tasks.domain.Entities;
using AutoMapper;

namespace autodoc.tasks.application.Mappings;

public sealed class TaskFileMappingProfile : Profile
{
	public TaskFileMappingProfile()
	{
		CreateMap<TaskFileEntity, TaskFileDto>();
		CreateMap<TaskFileEntity, TaskFileExtendedDto>();
	}
}