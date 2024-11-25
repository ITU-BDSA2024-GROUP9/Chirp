// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Chirp.Razor.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : Razor.Pages.Model
    {
        private readonly UserManager<Author> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            UserManager<Author> userManager,
            ILogger<PersonalDataModel> logger,
            ICheepService service) : base(service)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet([FromQuery] int page)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (page < 1) page = 1;
            SetUserVariables();
            if (userAuthor != null && userAuthor.UserName != null)
                PaginateCheepsByName(page, userAuthor.UserName);
            return Page();
        }
    }
}
