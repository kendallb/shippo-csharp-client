/*
 * Copyright 2011 - 2012 Xamarin, Inc., 2011 - 2012 Joe Dluzen
 *
 * Author(s):
 *  Gonzalo Paniagua Javier (gonzalo@xamarin.com)
 *  Joe Dluzen (jdluzen@gmail.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

namespace Shippo
{
    public interface IAPIResource : IAPIResourceBase
    {
        #region Address

        Address CreateAddress(Hashtable parameters);
        Address RetrieveAddress(String id);
        Address ValidateAddress(String id);
        ShippoCollection<Address> AllAddresss(Hashtable parameters);

        #region Parcel

        Parcel CreateParcel(Hashtable parameters);
        Parcel RetrieveParcel(String id);
        ShippoCollection<Parcel> AllParcels(Hashtable parameters);

        #endregion

        #region Shipment

        Shipment CreateShipment(Hashtable parameters);
        Shipment RetrieveShipment(String id);
        ShippoCollection<Shipment> AllShipments(Hashtable parameters);

        #endregion

        #endregion

        #region Rate

        ShippoCollection<Rate> CreateRate(Hashtable parameters);
        ShippoCollection<Rate> GetShippingRatesSync(String objectId);
        ShippoCollection<Rate> GetShippingRatesSync(Hashtable parameters);
        Rate RetrieveRate(String id);
        ShippoCollection<Rate> AllRates(Hashtable parameters);

        #endregion

        #region Transaction

        Transaction CreateTransaction(Hashtable parameters);
        Transaction CreateTransactionSync(Hashtable parameters);
        Transaction RetrieveTransaction(String id);
        ShippoCollection<Transaction> AllTransactions(Hashtable parameters);

        #endregion

        #region CustomsItem

        CustomsItem CreateCustomsItem(Hashtable parameters);
        CustomsItem RetrieveCustomsItem(String id);
        ShippoCollection<CustomsItem> AllCustomsItems(Hashtable parameters);

        #endregion

        #region CustomsDeclaration

        CustomsDeclaration CreateCustomsDeclaration(Hashtable parameters);
        CustomsDeclaration RetrieveCustomsDeclaration(String id);
        ShippoCollection<CustomsDeclaration> AllCustomsDeclarations(Hashtable parameters);

        #endregion

        #region CarrierAccount

        CarrierAccount CreateCarrierAccount(Hashtable parameters);
        CarrierAccount UpdateCarrierAccount(String object_id, Hashtable parameters);
        CarrierAccount RetrieveCarrierAccount(String object_id);
        ShippoCollection<CarrierAccount> AllCarrierAccount(Hashtable parameters);

        #endregion

        #region Refund

        Refund CreateRefund(Hashtable parameters);
        Refund RetrieveRefund(String id);
        ShippoCollection<Refund> AllRefunds(Hashtable parameters);

        #endregion

        #region Manifest

        Manifest CreateManifest(Hashtable parameters);
        Manifest RetrieveManifest(String id);
        ShippoCollection<Manifest> AllManifests(Hashtable parameters);

        #endregion
    }
}
