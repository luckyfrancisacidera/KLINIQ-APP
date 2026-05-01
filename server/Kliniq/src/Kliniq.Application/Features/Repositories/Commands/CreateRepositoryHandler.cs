//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Kliniq.Application.Features.Repositories.Commands
//{
//    public class CreateRepositoryHandler
//    {
//        private readonly IValidator<CreateRepositoryCommand> _validator;
//        private readonly IRepositoryRepository _repositoryRepository;
//        public CreateRepositoryHandler(IRepositoryRepository repositoryRepository, IValidator<CreateRepositoryCommand> validator)
//        {
//            _validator = validator;
//            _repositoryRepository = repositoryRepository;
//        }

//        public async Task<Repository> Handle(CreateRepositoryCommand command)
//        {
//            await _validator.ValidateAndThrowAsync(command);

//        }
//    }
//}
