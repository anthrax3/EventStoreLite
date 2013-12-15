﻿using System;
using EventStoreLite;
using Raven.Client;
using SampleDomain.Domain;
using SampleDomain.ViewModels;

namespace SampleDomain.Handlers
{
    public class CustomerHandler : IEventHandler<CustomerInitialized>,
                                   IEventHandler<CustomerNameChanged>
    {
        private readonly IDocumentSession session;

        public CustomerHandler(IDocumentSession session)
        {
            this.session = session;
            if (session == null) throw new ArgumentNullException("session");
        }

        public void Handle(CustomerInitialized e, string aggregateId)
        {
            var vm = session.Load<NamesViewModel>(NamesViewModel.DatabaseId);
            if (vm == null)
            {
                vm = new NamesViewModel();
                session.Store(vm);
            }

            vm.Names.Add(string.Format("New customer: {0}", e.Name));
        }

        public void Handle(CustomerNameChanged e, string aggregateId)
        {
            var vm = session.Load<NamesViewModel>(NamesViewModel.DatabaseId);
            if (vm == null)
            {
                vm = new NamesViewModel();
                session.Store(vm);
            }

            vm.Names.Add("Customer changed name to " + e.NewName);
            if (vm.Names.Count > 100)
                vm.Names.RemoveRange(0, vm.Names.Count - 100);
        }
    }
}