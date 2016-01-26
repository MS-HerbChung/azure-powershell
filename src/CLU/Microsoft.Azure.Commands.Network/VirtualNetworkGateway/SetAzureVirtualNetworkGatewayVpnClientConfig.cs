﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Management.Automation;
using AutoMapper;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Commands.Network.Models;

using MNM = Microsoft.Azure.Management.Network.Models;
using System.Collections;
using Microsoft.Azure.Commands.Tags.Model;
using System.Collections.Generic;

namespace Microsoft.Azure.Commands.Network
{
    [Cmdlet(VerbsCommon.Set, "AzureRmVirtualNetworkGatewayVpnClientConfig"), OutputType(typeof(PSVirtualNetworkGateway))]
    [CliCommandAlias("vnet gateway vpn client set")]
    public class SetAzureVirtualNetworkGatewayVpnClientConfigCommand : VirtualNetworkGatewayBaseCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The VirtualNetworkGateway")]
        [ValidateNotNull]
        public PSVirtualNetworkGateway VirtualNetworkGateway { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "P2S VpnClient AddressPool")]
        [ValidateNotNullOrEmpty]
        public List<string> VpnClientAddressPool { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            if (!this.IsVirtualNetworkGatewayPresent(this.VirtualNetworkGateway.ResourceGroupName, this.VirtualNetworkGateway.Name))
            {
                throw new ArgumentException(Microsoft.Azure.Commands.Network.Properties.Resources.ResourceNotFound);
            }

            if (this.VirtualNetworkGateway.VpnClientConfiguration == null)
            {
                this.VirtualNetworkGateway.VpnClientConfiguration = new PSVpnClientConfiguration();
            }
            this.VirtualNetworkGateway.VpnClientConfiguration.VpnClientAddressPool = new PSAddressSpace();
            this.VirtualNetworkGateway.VpnClientConfiguration.VpnClientAddressPool.AddressPrefixes = this.VpnClientAddressPool;

            // Map to the sdk object
            var virtualnetGatewayModel = Mapper.Map<MNM.VirtualNetworkGateway>(this.VirtualNetworkGateway);
            virtualnetGatewayModel.Tags = TagsConversionHelper.CreateTagDictionary(this.VirtualNetworkGateway.Tag, validate: true);

            this.VirtualNetworkGatewayClient.CreateOrUpdate(this.VirtualNetworkGateway.ResourceGroupName, this.VirtualNetworkGateway.Name, virtualnetGatewayModel);

            var getvirtualnetGateway = this.GetVirtualNetworkGateway(this.VirtualNetworkGateway.ResourceGroupName, this.VirtualNetworkGateway.Name);

            WriteObject(getvirtualnetGateway);
        }
    }
}
