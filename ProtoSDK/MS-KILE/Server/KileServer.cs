// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Microsoft.Protocols.TestTools.StackSdk.Security.Cryptographic;
using Microsoft.Protocols.TestTools.StackSdk.Transport;
using Microsoft.Protocols.TestTools.StackSdk.Asn1;
using Microsoft.Protocols.TestTools.StackSdk.Security.KerberosLib;

namespace Microsoft.Protocols.TestTools.StackSdk.Security.Kile
{
    /// <summary>
    /// The KILE server, receives PDUs from client and sends PDUs to client.
    /// It is called by test cases to create, send or receive PDUs.
    /// </summary>
    public class KileServer : KileRole
    {
        #region Private Members

        /// <summary>
        /// Represents whether this object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Contains all the important state variables in the context.
        /// Represents the context related to the currently operated client.
        /// </summary>
        internal KileServerContext context;

        /// <summary>
        /// A class to decode received PDUs.
        /// </summary>
        private KileDecoder decoder;

        /// <summary>
        /// A TCP/UDP transport instance, sending and receiving PDUs with the KDC.
        /// </summary>
        private TransportStack transport;

        /// <summary>
        /// The buffer size of transport stack.
        /// </summary>
        private int transportBufferSize;

        /// <summary>
        /// The realm part of the server's principal identifier.
        /// </summary>
        private string domain;

        /// <summary>
        /// Kile Server Context List
        /// </summary>
        private Dictionary<KileConnection, KileServerContext> contextList;

        /// <summary>
        /// Read only Kile Server Context List
        /// </summary>
        private ReadOnlyDictionary<KileConnection, KileServerContext> readOnlyContextList;

        #endregion members


        #region Properties

        /// <summary>
        /// Contains all the important state variables in the context.
        /// </summary>
        public ReadOnlyDictionary<KileConnection, KileServerContext> ContextList
        {
            get
            {
                return readOnlyContextList;
            }
        }


        /// <summary>
        /// The buffer size of transport stack. The default value is 1500.
        /// </summary>
        public int TransportStackBufferSize
        {
            get
            {
                return transportBufferSize;
            }
        }


        /// <summary>
        /// Contains all the important state variables in the context.
        /// Represents the context related to the currently operated client.
        /// </summary>
        internal override KileContext Context
        {
            get
            {
                return context;
            }
        }
        #endregion


        #region Constructor

        /// <summary>
        /// Create a KileServer instance.
        /// </summary>
        /// <param name="domain">The realm part of the server's principal identifier.
        /// This argument cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        public KileServer(string domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            contextList = new Dictionary<KileConnection, KileServerContext>(new KileServerContextComparer());
            readOnlyContextList = new ReadOnlyDictionary<KileConnection, KileServerContext>(contextList);
            transportBufferSize = ConstValue.TRANSPORT_BUFFER_SIZE;
            this.domain = domain;
        }

        #endregion


        #region Packet API

        /// <summary>
        /// Create AS response.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client. This argument cannot be null.</param>
        /// <param name="accountType">The type of the logon account. User or Computer</param>
        /// <param name="password">Password of the user who logon the system. This argument cannot be null.</param>
        /// <param name="SeqofPaData">The pre-authentication data in AS request. 
        /// This argument can be generated by method ConstructPaData. This argument could be null.</param> 
        /// <param name="encTicketFlags">Ticket Flags</param>
        /// <param name="ticketAuthorizationData">The authorization-data field is used to pass authorization data from
        /// the principal on whose behalf a ticket was issued to the application service. This parameter could be null.
        /// </param>
        /// <returns>The created AS response.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when no kileConnection related server context
        /// is found </exception>
        [CLSCompliant(false)]
        public KileAsResponse CreateAsResponse(
            KileConnection kileConnection,
            KileAccountType accountType,
            string password,
            Asn1SequenceOf<PA_DATA> SeqofPaData,
            EncTicketFlags encTicketFlags,
            AuthorizationData ticketAuthorizationData)
        {
            KileServerContext serverContext = GetServerContextByKileConnection(kileConnection);
            string cName = serverContext.UserName.name_string.Elements[0].Value;
            string cRealm = serverContext.UserRealm.Value;
            serverContext.Salt = GenerateSalt(cRealm, cName, accountType);
            serverContext.TicketEncryptKey = new EncryptionKey(new KerbInt32((int)EncryptionType.RC4_HMAC),
                new Asn1OctetString(GetEncryptionKeyByType(EncryptionType.RC4_HMAC)));

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            else
            {
                serverContext.Password = password;
            }
            KileAsResponse response = new KileAsResponse(serverContext);

            // Construct a Ticket
            Ticket ticket = new Ticket();
            ticket.tkt_vno = new Asn1Integer(ConstValue.KERBEROSV5);
            ticket.realm = new Realm(domain);
            ticket.sname = serverContext.SName;

            // Set EncTicketPart
            EncTicketPart encTicketPart = new EncTicketPart();
            EncryptionType encryptionType = (EncryptionType)serverContext.EncryptType.Elements[0].Value;

            encTicketPart.key = new EncryptionKey(new KerbInt32((int)encryptionType), new Asn1OctetString(GetEncryptionKeyByType(encryptionType)));
            encTicketPart.flags = new TicketFlags(KileUtility.ConvertInt2Flags((int)encTicketFlags));
            encTicketPart.crealm = serverContext.UserRealm;
            encTicketPart.cname = serverContext.UserName;
            encTicketPart.transited = new TransitedEncoding(new KerbInt32(4), null);
            encTicketPart.authtime = KileUtility.CurrentKerberosTime;
            encTicketPart.starttime = KileUtility.CurrentKerberosTime;
            encTicketPart.endtime = serverContext.endTime;
            encTicketPart.renew_till = serverContext.rtime ?? encTicketPart.endtime;
            encTicketPart.caddr = serverContext.Addresses;
            encTicketPart.authorization_data = ticketAuthorizationData;
            response.TicketEncPart = encTicketPart;

            // Set AS_REP
            response.Response.pvno = new Asn1Integer(ConstValue.KERBEROSV5);
            response.Response.msg_type = new Asn1Integer((int)MsgType.KRB_AS_RESP);
            response.Response.padata = SeqofPaData;
            response.Response.crealm = serverContext.UserRealm;
            response.Response.cname = serverContext.UserName;
            response.Response.ticket = ticket;

            // Set EncASRepPart
            EncASRepPart encASRepPart = new EncASRepPart();
            encASRepPart.key = encTicketPart.key;
            LastReqElement element = new LastReqElement(new KerbInt32(0), KileUtility.CurrentKerberosTime);
            encASRepPart.last_req = new LastReq(new LastReqElement[] { element });
            encASRepPart.nonce = serverContext.Nonce;
            encASRepPart.flags = encTicketPart.flags;
            encASRepPart.authtime = encTicketPart.authtime;
            encASRepPart.starttime = encTicketPart.starttime;
            encASRepPart.endtime = encTicketPart.endtime;
            encASRepPart.renew_till = encTicketPart.renew_till;
            encASRepPart.srealm = ticket.realm;
            encASRepPart.sname = ticket.sname;
            encASRepPart.caddr = encTicketPart.caddr;
            response.EncPart = encASRepPart;

            return response;
        }


        /// <summary>
        /// Get encryption key by the encryption type
        /// </summary>
        /// <param name="encryptionType">The specified encryption type</param>
        /// <returns>Encryption key</returns>
        private byte[] GetEncryptionKeyByType(EncryptionType encryptionType)
        {
            byte[] key;

            if (encryptionType == EncryptionType.AES256_CTS_HMAC_SHA1_96)
            {
                key = ArrayUtility.ConcatenateArrays(Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray());
            }
            else if (encryptionType == EncryptionType.DES_CBC_CRC || encryptionType == EncryptionType.DES_CBC_MD5)
            {
                key = ArrayUtility.SubArray(Guid.NewGuid().ToByteArray(), ConstValue.BYTE_SIZE);
            }
            else
            {
                key = Guid.NewGuid().ToByteArray();
            }

            return key;
        }


        /// <summary>
        /// Create TGS response.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client. This argument cannot be null.</param>
        /// <param name="seqOfPaData">The pre-authentication data.
        /// This argument can be generated by method ConstructPaData. This argument could be null.</param> 
        /// <param name="encTicketFlags">Ticket Flags</param>
        /// <param name="ticketEncryptKey">Encryption key used to encrypt ticket. This parameter cannot be null
        /// In User-User Authentication mode, use session key in additional ticket in KileTgsRequest.
        /// Otherwise use service's secret key.</param>
        /// <param name="ticketAuthorizationData">The authorization-data field is used to pass authorization data from
        /// the principal on whose behalf a ticket was issued to the application service. This parameter could be null.
        /// </param>
        /// <returns>The created TGS response.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when no kileConnection related server context
        /// is found </exception>
        [CLSCompliant(false)]
        public KileTgsResponse CreateTgsResponse(
            KileConnection kileConnection,
            Asn1SequenceOf<PA_DATA> seqOfPaData,
            EncTicketFlags encTicketFlags,
            EncryptionKey ticketEncryptKey,
            AuthorizationData ticketAuthorizationData)
        {
            KileServerContext serverContext = GetServerContextByKileConnection(kileConnection);

            if (ticketEncryptKey == null)
            {
                throw new ArgumentNullException("ticketEncryptKey");
            }
            else
            {
                serverContext.TicketEncryptKey = ticketEncryptKey;
            }
            KileTgsResponse response = new KileTgsResponse(serverContext);

            // Construct a Ticket
            Ticket ticket = new Ticket();
            ticket.tkt_vno = new Asn1Integer(ConstValue.KERBEROSV5);
            ticket.realm = new Realm(domain);
            ticket.sname = serverContext.SName;

            // Set EncTicketPart
            EncTicketPart encTicketPart = new EncTicketPart();
            EncryptionType encryptionType = (EncryptionType)serverContext.EncryptType.Elements[0].Value;
            encTicketPart.key = new EncryptionKey(new KerbInt32((int)encryptionType), new Asn1OctetString(GetEncryptionKeyByType(encryptionType)));
            encTicketPart.flags = new TicketFlags(KileUtility.ConvertInt2Flags((int)encTicketFlags));
            encTicketPart.crealm = serverContext.TgsTicket.crealm;
            encTicketPart.cname = serverContext.TgsTicket.cname;
            encTicketPart.transited = serverContext.TgsTicket.transited;
            encTicketPart.authtime = KileUtility.CurrentKerberosTime;
            encTicketPart.starttime = KileUtility.CurrentKerberosTime;
            encTicketPart.endtime = serverContext.TgsTicket.endtime;
            encTicketPart.renew_till = serverContext.TgsTicket.renew_till;
            encTicketPart.caddr = serverContext.Addresses;
            encTicketPart.authorization_data = ticketAuthorizationData;
            response.TicketEncPart = encTicketPart;

            // Set AS_REP
            response.Response.pvno = new Asn1Integer(ConstValue.KERBEROSV5);
            response.Response.msg_type = new Asn1Integer((int)MsgType.KRB_TGS_RESP);
            response.Response.padata = seqOfPaData;
            response.Response.crealm = serverContext.UserRealm;
            response.Response.cname = serverContext.UserName;
            response.Response.ticket = ticket;

            // Set EncASRepPart
            EncTGSRepPart encTGSRepPart = new EncTGSRepPart();
            encTGSRepPart.key = encTicketPart.key;
            LastReqElement element = new LastReqElement(new KerbInt32(0), KileUtility.CurrentKerberosTime);
            encTGSRepPart.last_req = new LastReq(new LastReqElement[] { element });
            encTGSRepPart.nonce = serverContext.Nonce;
            encTGSRepPart.flags = encTicketPart.flags;
            encTGSRepPart.authtime = encTicketPart.authtime;
            encTGSRepPart.starttime = encTicketPart.starttime;
            encTGSRepPart.endtime = encTicketPart.endtime;
            encTGSRepPart.renew_till = encTicketPart.renew_till;
            encTGSRepPart.srealm = ticket.realm;
            encTGSRepPart.sname = ticket.sname;
            encTGSRepPart.caddr = encTicketPart.caddr;
            response.EncPart = encTGSRepPart;

            return response;
        }


        /// <summary>
        /// Create TGS response.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client. This argument cannot be null.</param>
        /// <param name="seqOfPaData">The pre-authentication data.
        /// This argument can be generated by method ConstructPaData. This argument could be null.</param> 
        /// <param name="encTicketFlags">Ticket Flags</param>
        /// <param name="ticketEncryptKey">Encryption key used to encrypt ticket. This parameter cannot be null
        /// In User-User Authentication mode, use session key in additional ticket in KileTgsRequest.
        /// Otherwise use service's secret key.
        /// A 16 byte buffer. RC4-HMAC encryption type is used for this key</param>
        /// <returns>The created TGS response.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when no kileConnection related server context
        /// is found </exception>
        [CLSCompliant(false)]
        public KileTgsResponse CreateTgsResponse(
            KileConnection kileConnection,
            Asn1SequenceOf<PA_DATA> seqOfPaData,
            EncTicketFlags encTicketFlags,
            byte[] ticketEncryptKey)
        {
            if (ticketEncryptKey == null)
            {
                throw new ArgumentNullException("ticketEncryptKey");
            }
            EncryptionKey ticketKey = new EncryptionKey(new KerbInt32((int)EncryptionType.RC4_HMAC), new Asn1OctetString(ticketEncryptKey));
            return CreateTgsResponse(kileConnection, seqOfPaData, encTicketFlags, ticketKey, null);
        }


        /// <summary>
        /// Create AP response. This method is used for mutual authentication.
        /// Then use KilePdu.ToBytes() to get the byte array.
        /// This method is used to create ApResponse on server side.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client. This argument cannot be null.</param>
        /// <param name="subkey">
        /// Specify the new subkey used in the following exchange.
        /// If this argument is null, no subkey will be sent.</param>
        /// <returns>The created AP response.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when no kileConnection related server context
        /// is found </exception>
        [CLSCompliant(false)]
        public KileApResponse CreateApResponse(KileConnection kileConnection, EncryptionKey subkey)
        {
            context = GetServerContextByKileConnection(kileConnection);
            KileApResponse apResponse = CreateApResponse(subkey);

            // Set a random sequence number
            Random randomNumber = new Random();
            apResponse.ApEncPart.seq_number = new KerbUInt32(randomNumber.Next());
            context.currentLocalSequenceNumber = (ulong)apResponse.ApEncPart.seq_number.Value;

            return apResponse;
        }


        /// <summary>
        /// Create KRB_ERROR response
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client. This argument cannot be null.</param>
        /// <param name="errorCode">Error code returned by Kerberos or the server when a request fails</param>
        /// <param name="errorText">Additional text to help explain the error code. This argument could be null.</param>
        /// <param name="errorData">Additional data about the error. This argument could be null.</param>
        /// <returns>The created Krb Error response</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        public KileKrbError CreateKrbErrorResponse(
            KileConnection kileConnection,
            KRB_ERROR_CODE errorCode,
            string errorText,
            byte[] errorData)
        {
            KileServerContext serverContext = GetServerContextByKileConnection(kileConnection);
            KileKrbError response = new KileKrbError(serverContext);

            // Set KRB_ERROR
            response.KerberosError.pvno = new Asn1Integer(ConstValue.KERBEROSV5);
            response.KerberosError.msg_type = new Asn1Integer((int)MsgType.KRB_ERROR);
            response.KerberosError.stime = KileUtility.CurrentKerberosTime;
            response.KerberosError.susec = new Microseconds(0);
            response.KerberosError.sname = serverContext.SName;
            response.KerberosError.realm = new Realm(domain);
            response.KerberosError.error_code = new KerbInt32((int)errorCode);

            if (errorText != null)
            {
                response.KerberosError.e_text = new KerberosString(errorText);
            }
            if (errorData != null)
            {
                response.KerberosError.e_data = new Asn1OctetString(errorData);
            }

            return response;
        }

        #endregion


        #region Wrap/Unwrap, GetMic/VerifyMic

        /// <summary>
        /// Create a Gss_Wrap token. Then use KilePdu.ToBytes() to get the byte array.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <param name="isEncrypted">If encrypt the message.</param>
        /// <param name="signAlgorithm">Specify the checksum type.
        /// This is only used for encryption types DES and RC4.</param>
        /// <param name="message">The message to be wrapped. This argument can be null.</param>
        /// <returns>The created Gss_Wrap token.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when the encryption is not supported.</exception>
        [CLSCompliant(false)]
        public KilePdu GssWrap(KileConnection kileConnection, bool isEncrypted, SGN_ALG signAlgorithm, byte[] message)
        {
            context = GetServerContextByKileConnection(kileConnection);
            return GssWrap(isEncrypted, signAlgorithm, message);
        }


        /// <summary>
        /// Create a Gss_GetMic token. Then use KilePdu.ToBytes() to get the byte array.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <param name="signAlgorithm">Specify the checksum type.
        /// This is only used for encryption types DES and RC4.</param>
        /// <param name="message">The message to be computed signature. This argument can be null.</param>
        /// <returns>The created Gss_GetMic token, NotSupportedException.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when the encryption is not supported.</exception>
        [CLSCompliant(false)]
        public KilePdu GssGetMic(KileConnection kileConnection, SGN_ALG signAlgorithm, byte[] message)
        {
            context = GetServerContextByKileConnection(kileConnection);
            return GssGetMic(signAlgorithm, message);
        }


        /// <summary>
        /// Decode a Gss_Wrap token.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <param name="token">The token got from an application message. 
        /// If this argument is null, null will be returned.</param>
        /// <returns>The decoded Gss_Wrap token.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when the encryption is not supported.</exception>
        public KilePdu GssUnWrap(KileConnection kileConnection, byte[] token)
        {
            context = GetServerContextByKileConnection(kileConnection);
            return GssUnWrap(token);
        }


        /// <summary>
        /// Decode and verify a Gss_GetMic token.
        /// </summary>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <param name="token">The token got from an application message. 
        /// If this argument is null, null will be returned.</param>
        /// <param name="message">The message to be computed signature. 
        /// If this argument is null, null will be returned.</param>
        /// <param name="pdu">The decoded Gss_GetMic token.</param>
        /// <returns>If verifying mic token is successful.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when the encryption is not supported.</exception>
        public bool GssVerifyMic(KileConnection kileConnection, byte[] token, byte[] message, out KilePdu pdu)
        {
            context = GetServerContextByKileConnection(kileConnection);
            return GssVerifyMic(token, message, out pdu);
        }

        #endregion


        #region Transport Methods

        /// <summary>
        /// Set up the TCP/UDP transport connection with KDC.
        /// </summary>
        /// <param name="localPort">The server port</param>
        /// <param name="transportType">Whether the transport is TCP or UDP transport.</param>
        [CLSCompliant(false)]
        public void Start(ushort localPort, KileConnectionType transportType)
        {
            Start(localPort, transportType, KileIpType.Ipv6, transportBufferSize);
        }


        /// <summary>
        /// Set up the TCP/UDP transport connection with KDC.
        /// </summary>
        /// <param name="localPort">The server port</param>
        /// <param name="transportType">Whether the transport is TCP or UDP transport.</param>
        /// <param name="ipType">Ip Version</param>
        /// <param name="transportSize">The buffer size of transport stack. </param>
        /// <exception cref="System.ArgumentException">Thrown when the transportType is neither TCP nor UDP.</exception>
        [CLSCompliant(false)]
        public void Start(ushort localPort, KileConnectionType transportType, KileIpType ipType, int transportSize)
        {
            SocketTransportConfig transportConfig = new SocketTransportConfig();
            transportConfig.Role = Role.Server;
            transportConfig.MaxConnections = ConstValue.MAX_CONNECTIONS;
            transportConfig.BufferSize = transportSize;

            if (ipType == KileIpType.Ipv4)
            {
                transportConfig.LocalIpAddress = IPAddress.Any;
            }
            else
            {
                transportConfig.LocalIpAddress = IPAddress.IPv6Any;
            }
            transportConfig.LocalIpPort = localPort;

            if (transportType == KileConnectionType.TCP)
            {
                transportConfig.Type = StackTransportType.Tcp;
            }
            else if (transportType == KileConnectionType.UDP)
            {
                transportConfig.Type = StackTransportType.Udp;
            }
            else
            {
                throw new ArgumentException("ConnectionType can only be TCP or UDP.");
            }
            decoder = new KileDecoder(contextList, transportType);
            transport = new TransportStack(transportConfig, decoder.DecodePacketCallback);
            transport.Start();
        }


        /// <summary>
        /// Wait for a connection or disconnection from client
        /// </summary>
        /// <param name="timeout">Max time for waiting</param>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the transport event is neither connected nor disconnected.</exception>
        public void ExpectConnection(TimeSpan timeout, out KileConnection kileConnection)
        {
            TransportEvent transEvent = transport.ExpectTransportEvent(timeout);

            if (transEvent.EventType != EventType.Connected && transEvent.EventType != EventType.Disconnected)
            {
                throw new InvalidOperationException("Reveived an un-expected transport event");
            }
            IPEndPoint ipEndPoint = (IPEndPoint)transEvent.EndPoint;
            kileConnection = new KileConnection(ipEndPoint);
        }


        /// <summary>
        /// Encode a PDU to a binary stream. Then send the stream.
        /// </summary>
        /// <param name="pdu">A specified type of a PDU. This argument cannot be null.</param>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        public void SendPdu(KilePdu pdu, KileConnection kileConnection)
        {
            if (kileConnection == null)
            {
                throw new ArgumentNullException("kileConnection");
            }
            if (pdu == null)
            {
                throw new ArgumentNullException("pdu");
            }

            KileServerContext serverContext = GetServerContextByKileConnection(kileConnection);
            transport.SendPacket(kileConnection.TargetEndPoint, pdu);
            serverContext.UpdateContext(pdu);
        }


        /// <summary>
        /// Send a byte array to client. This method is especially used for negative test.
        /// </summary>
        /// <param name="packetBuffer">The bytes to be sent. This argument cannot be null.</param>
        /// <param name="kileConnection">Maintain a connection with a target client</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        public void SendBytes(byte[] packetBuffer, KileConnection kileConnection)
        {
            if (kileConnection == null)
            {
                throw new ArgumentNullException("kileConnection");
            }
            if (packetBuffer == null)
            {
                throw new ArgumentNullException("packetBuffer");
            }

            transport.SendBytes(kileConnection.TargetEndPoint, packetBuffer);
        }


        /// <summary>
        /// Expect to receive a PDU of any type from the remote host.
        /// </summary>
        /// <param name="timeout">Timeout of receiving PDU.</param>
        /// <param name="kileConnection">The connection with a client.</param>
        /// <returns>The expected PDU.</returns>
        public KilePdu ExpectPdu(TimeSpan timeout, out KileConnection kileConnection)
        {
            TransportEvent eventPacket = transport.ExpectTransportEvent(timeout);
            KilePdu packet = (KilePdu)eventPacket.EventObject;
            kileConnection = new KileConnection((IPEndPoint)eventPacket.EndPoint);

            return packet;
        }


        /// <summary>
        /// Disconnect with a target client.
        /// </summary>
        /// <param name="kileConnection">The connection with the target client.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        public void Disconnect(KileConnection kileConnection)
        {
            if (kileConnection == null)
            {
                throw new ArgumentNullException("kileConnection");
            }
            transport.Disconnect(kileConnection.TargetEndPoint);
            DeleteServerContextByConnection(kileConnection);
        }

        #endregion


        #region Context Operation

        /// <summary>
        /// Get a server context for a specified client's connection.
        /// </summary>
        /// <param name="kileConnection">The specified connection.
        /// Null will be returned if this parameter is null.</param>
        /// <returns>The specified server context</returns>
        public KileServerContext GetServerContextByConnection(KileConnection kileConnection)
        {
            KileServerContext serverContext = null;

            if (kileConnection != null && contextList.ContainsKey(kileConnection))
            {
                serverContext = contextList[kileConnection];
            }
            return serverContext;
        }


        /// <summary>
        /// Delete a server context for a specified client's connection.
        /// </summary>
        /// <param name="kileConnection">The specified connection. This argument cannot be null.</param>
        /// <returns>True if the element is successfully found and removed; otherwise, false.
        /// This method returns false if key is not found</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the input parameter is null.</exception>
        public bool DeleteServerContextByConnection(KileConnection kileConnection)
        {
            if (kileConnection == null)
            {
                throw new ArgumentNullException("kileConnection");
            }
            return contextList.Remove(kileConnection);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get a server context for a specified client's connection.
        /// </summary>
        /// <param name="kileConnection">The specified connection.</param>
        /// <returns>Specified server context if existed and not null</returns>
        private KileServerContext GetServerContextByKileConnection(KileConnection kileConnection)
        {
            if (kileConnection == null)
            {
                throw new ArgumentNullException("kileConnection");
            }
            if (!contextList.ContainsKey(kileConnection))
            {
                throw new InvalidOperationException("The specified kileConnection does not exist.");
            }
            KileServerContext serverContext = contextList[kileConnection];

            if (serverContext == null)
            {
                throw new InvalidOperationException("The kileConnection related context does not exist.");
            }
            return serverContext;
        }

        #endregion


        #region IDisposable

        /// <summary>
        /// Release resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, Managed and unmanaged resources are disposed.
        /// if false, Only unmanaged resources can be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //Release managed resource.
                    if (transport != null)
                    {
                        transport.Dispose();
                        transport = null;
                    }
                }

                //Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }
}
