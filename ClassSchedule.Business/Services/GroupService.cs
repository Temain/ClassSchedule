﻿using System;
using System.Collections.Generic;
using System.Linq;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Business.Services
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;

        public GroupService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает список групп, для которых редактируется расписание
        /// </summary>
        public IQueryable<Group> GetEditableGroups(string userId)
        {
            var groups = _context.GroupSets
                .Where(x => x.IsSelected && x.ApplicationUserId == userId)
                .SelectMany(x => x.GroupSetGroups)
                .OrderBy(x => x.Order)
                .Select(x => x.Group);

            return groups;
        }

        /// <summary>
        /// Возвращает список идентификаторов групп, для которых редактируется расписание
        /// </summary>
        public List<int> GetEditableGroupsIdentifiers(string userId)
        {
            var groups = _context.GroupSets
                .Where(x => x.IsSelected && x.ApplicationUserId == userId)
                .SelectMany(x => x.GroupSetGroups)
                .OrderBy(x => x.Order)
                .Select(x => x.Group.GroupId)
                .ToList();

            return groups;
        }
    }
}