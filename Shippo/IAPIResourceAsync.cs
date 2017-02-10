/*
 * Copyright 2011 - 2012 Xamarin, Inc., 2011 - 2012 Joe Dluzen
 *
 * AuthorAsync(s):
 *  Gonzalo Paniagua Javier Async(gonzalo@xamarin.com)
 *  Joe Dluzen Async(jdluzen@gmail.com)
 *
 * Licensed under the Apache License, Version 2.0 Async(the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Threading.Tasks;

namespace Shippo
{
    public interface IAPIResourceAsync : IAPIResourceBase
    {
        #region Address

        Task<Address> CreateAddressAsync(Hashtable parameters);
        Task<Address> RetrieveAddressAsync(String id);
        Task<Address> ValidateAddressAsync(String id);
        Task<ShippoCollection<Address>> AllAddresssAsync(Hashtable parameters);

        #endregion

        #region Parcel

        Task<Parcel> CreateParcelAsync(Hashtable parameters);
        Task<Parcel> RetrieveParcelAsync(String id);
        Task<ShippoCollection<Parcel>> AllParcelsAsync(Hashtable parameters);

        #endregion

        #region Shipment

        Task<Shipment> CreateShipmentAsync(Hashtable parameters);
        Task<Shipment> RetrieveShipmentAsync(String id);
        Task<ShippoCollection<Shipment>> AllShipmentsAsync(Hashtable parameters);

        #endregion

        #region Rate

        Task<ShippoCollection<Rate>> CreateRateAsync(Hashtable parameters);
        Task<ShippoCollection<Rate>> GetShippingRatesSyncAsync(String objectId);
        Task<ShippoCollection<Rate>> GetShippingRatesSyncAsync(Hashtable parameters);
        Task<Rate> RetrieveRateAsync(String id);
        Task<ShippoCollection<Rate>> AllRatesAsync(Hashtable parameters);

        #endregion

        #region Transaction

        Task<Transaction> CreateTransactionAsync(Hashtable parameters);
        Task<Transaction> CreateTransactionSyncAsync(Hashtable parameters);
        Task<Transaction> RetrieveTransactionAsync(String id);
        Task<ShippoCollection<Transaction>> AllTransactionsAsync(Hashtable parameters);

        #endregion

        #region CustomsItem

        Task<CustomsItem> CreateCustomsItemAsync(Hashtable parameters);
        Task<CustomsItem> RetrieveCustomsItemAsync(String id);
        Task<ShippoCollection<CustomsItem>> AllCustomsItemsAsync(Hashtable parameters);

        #endregion

        #region CustomsDeclaration

        Task<CustomsDeclaration> CreateCustomsDeclarationAsync(Hashtable parameters);
        Task<CustomsDeclaration> RetrieveCustomsDeclarationAsync(String id);
        Task<ShippoCollection<CustomsDeclaration>> AllCustomsDeclarationsAsync(Hashtable parameters);

        #endregion

        #region CarrierAccount

        Task<CarrierAccount> CreateCarrierAccountAsync(Hashtable parameters);
        Task<CarrierAccount> UpdateCarrierAccountAsync(String object_id, Hashtable parameters);
        Task<CarrierAccount> RetrieveCarrierAccountAsync(String object_id);
        Task<ShippoCollection<CarrierAccount>> AllCarrierAccountAsync(Hashtable parameters);

        #endregion

        #region Refund

        Task<Refund> CreateRefundAsync(Hashtable parameters);
        Task<Refund> RetrieveRefundAsync(String id);
        Task<ShippoCollection<Refund>> AllRefundsAsync(Hashtable parameters);

        #endregion

        #region Manifest

        Task<Manifest> CreateManifestAsync(Hashtable parameters);
        Task<Manifest> RetrieveManifestAsync(String id);
        Task<ShippoCollection<Manifest>> AllManifestsAsync(Hashtable parameters);

        #endregion
    }
}
