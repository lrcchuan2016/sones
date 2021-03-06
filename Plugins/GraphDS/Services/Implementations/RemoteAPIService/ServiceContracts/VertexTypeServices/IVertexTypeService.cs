﻿/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices
{
	[ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "VertexTypeService")]
	public interface IVertexTypeService
	{
		#region IBaseTypeServices

		#region Inheritance

		/// <summary>
		/// Returns the descendant of this IVertexType.
		/// </summary>
		/// <returns>An enumeration of IVertexType that are descendant of this IVertexType.</returns>
		/// <seealso cref="IBaseTypeService.GetDescendantTypes"/>
		[OperationContract]
		List<ServiceVertexType> GetDescendantVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns the descendant of this IVertexType and this IVertexType in one enumeration.
		/// </summary>
		/// <returns>An enumeration of IVertexType that are descendant of this IVertexType and this IVertexType itself.</returns>
		/// <seealso cref="IBaseTypeService.GetDescendantTypesAndSelf"/>
		[OperationContract]
		List<ServiceVertexType> GetDescendantVertexTypesAndSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns the ancestor of this IVertexType.
		/// </summary>
		/// <returns>An enumeration of IVertexType that are ancestors of this IVertexType.</returns>
		/// <seealso cref="IBaseTypeService.GetAncestorTypes"/>
		[OperationContract]
		List<ServiceVertexType> GetAncestorVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns the ancestor of this IVertexType and this IVertexType in one enumeration.
		/// </summary>
		/// <returns>An enumeration of IVertexType that are ancestors of this IVertexType and this IVertexType itself.</returns>
		/// <seealso cref="IBaseTypeService.GetAncestorTypesAndSelf"/>
		[OperationContract]
		List<ServiceVertexType> GetAncestorVertexTypesAndSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns all descendant and ancestors of this IVertexType.
		/// </summary>
		/// <returns>An enumeration of all IVertexType that are ancestors or descendant of this IVertexType.</returns>
		/// <seealso cref="IBaseTypeService.GetKinsmenTypes"/>
		[OperationContract]
		List<ServiceVertexType> GetKinsmenVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns all descendant and ancestors of this IVertexType and this IVertexType in one enumeration. 
		/// </summary>
		/// <returns>An enumeration of all IVertexType that are ancestors or descendant of this IVertexType and this IVertexType itself.</returns>
		/// <seealso cref="IBaseTypeService.GetKinsmenTypesAndSelf"/>
		[OperationContract]
		List<ServiceVertexType> GetKinsmenVertexTypesAndSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns the direct children of this IVertexType.
		/// </summary>
		/// <seealso cref="IBaseTypeService.ChildrenTypes"/>
		[OperationContract]
		List<ServiceVertexType> ChildrenVertexTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Gets the parent of this IVertexType.
		/// </summary>
		/// <seealso cref="IBaseTypeService.ParentType"/>
		[OperationContract]
		ServiceVertexType ParentVertexType(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		#endregion

		#region Inheritance

		/// <summary>
		/// Defines whether this type can be used as parent type.
		/// </summary>
		/// <value>
		/// If true, this vertex type must not be used as a parent vertex type.
		/// </value>
		[OperationContract(Name = "IsSealedByVertexType")]
		Boolean IsSealed(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Has this type a parent type?
		/// </summary>
		/// <returns>True, if this type has a parent type, otherwise false.</returns>
		[OperationContract(Name = "HasParentTypeByVertexType")]
		bool HasParentType(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Has this type child types?
		/// </summary>
		/// <returns>False, if this type has no child types, otherwise true.</returns>
		[OperationContract(Name = "HasChildTypeByVertexType")]
		bool HasChildTypes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType);

		/// <summary>
		/// Returns if the given type is an ancestor of the current type.
		/// </summary>
		/// <param name="myOtherType">The given type.</param>
		/// <returns>True, if the given type is an ancestor of the current type, otherwise false.</returns>
		[OperationContract(Name = "IsAncestorByVertexType")]
		bool IsAncestor(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType);

		/// <summary>
		/// Returns if the given type is an ancestor of or the current itself.
		/// </summary>
		/// <param name="myOtherType">The given type.</param>
		/// <returns>True, if the given type is an ancestor of the current type or the current type itself, otherwise false.</returns>
		[OperationContract(Name = "IsAncestorOrSelfByVertexType")]
		bool IsAncestorOrSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType);

		/// <summary>
		/// Returns if the given type is a descendant of the current type.
		/// </summary>
		/// <param name="myOtherType">The given type.</param>
		/// <returns>True, if the given type is a descendant of the current type, otherwise false.</returns>
		[OperationContract(Name = "IsDescendantByVertexType")]
		bool IsDescendant(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType);

		/// <summary>
		/// Returns if the given type is a descendant of or the current type itself.
		/// </summary>
		/// <param name="myOtherType">The given type.</param>
		/// <returns>True, if the given type is a descendant of the current type or the current type itself, otherwise false.</returns>
		[OperationContract(Name = "IsDescendantOrSelfByVertexType")]
		bool IsDescendantOrSelf(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, ServiceVertexType myOtherType);

		#endregion

		#region Attributes

		/// <summary>
		/// Has this vertex type a certain attribute?
		/// </summary>
		/// <returns>True or false</returns>
		[OperationContract(Name = "HasAttributeByVertexType")]
		bool HasAttribute(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myAttributeName);

		/// <summary>
		/// Gets a certain attribute definition
		/// </summary>
		/// <param name="myAttributeName">The name of the interesting attribute</param>
		/// <returns>A attribute definition</returns>
		[OperationContract(Name = "GetAttributeDefinitionByVertexType")]
		ServiceAttributeDefinition GetAttributeDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myAttributeName);

		/// <summary>
		/// Gets a certain attribute definition
		/// </summary>
		/// <param name="myAttributeID">The id of the interesting attribute</param>
		/// <returns>A attribute definition</returns>
		[OperationContract(Name = "GetAttributeDefinitionByIDByVertexType")]
		ServiceAttributeDefinition GetAttributeDefinitionByID(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, Int64 myAttributeID);

		/// <summary>
		/// Has this vertex type any attributes?
		/// </summary>
		/// <returns>True or false</returns>
		[OperationContract(Name = "HasAttributesByVertexType")]
		bool HasAttributes(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// Gets all attributes defined on this vertex type.
		/// </summary>
		/// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
		/// <returns>An enumerable of attribute definitions</returns>
		[OperationContract(Name = "GetAttributeDefinitionsByVertexType")]
		List<ServiceAttributeDefinition> GetAttributeDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		#endregion

		#region Properties

		/// <summary>
		/// Has this vertex type a certain property?
		/// </summary>
		/// <returns>True or false</returns>
		[OperationContract(Name = "HasPropertyByVertexType")]
		bool HasProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myAttributeName);

		/// <summary>
		/// Gets a certain attribute definition
		/// </summary>
		/// <param name="myPropertyName">The name of the property</param>
		/// <returns>A property definition</returns>
		[OperationContract]
        ServicePropertyDefinition GetPropertyDefinitionByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, String myPropertyName);

		/// <summary>
		/// Gets a certain attribute definition
		/// </summary>
		/// <param name="myPropertyID">The id of the property</param>
		/// <returns>A property definition</returns>
		[OperationContract]
        ServicePropertyDefinition GetPropertyDefinitionByIDByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, Int64 myPropertyID);

		/// <summary>
		/// Has this vertex type any properties?
		/// </summary>
		/// <returns>True or false</returns>
		[OperationContract(Name = "HasPropertiesByVertexType")]
		bool HasProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// Gets all properties defined on this vertex type.
		/// </summary>
		/// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
		/// <returns>An enumerable of property definitions</returns>
		[OperationContract]
        List<ServicePropertyDefinition> GetPropertyDefinitionsByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// Gets the properties with the given name.
		/// </summary>
		/// <param name="myPropertyNames">A list of peroperty names.</param>
		/// <returns>An enumerable of property definitions</returns>
		[OperationContract]
        List<ServicePropertyDefinition> GetPropertyDefinitionsByNameListByVertexType(SecurityToken mySecurityToken, Int64 myTransToken, String myServiceVertexTypeName, List<string> myPropertyNames);
		
		#endregion

		#endregion

		#region Incoming

		/// <summary>
		/// Has this vertex type a certain binary property?
		/// </summary>
		/// <param name="myPropertyName">The name of the binary property.</param>
		/// <returns>True, if a binary property with the given name exists, otherwise false.</returns>
        [OperationContract]
		bool HasBinaryProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myPropertyName);

		/// <summary>
		/// Gets a certain binary property definition.
		/// </summary>
		/// <param name="myPropertyName">The name of the interesting binary property.</param>
		/// <returns>A binary property definition, if existing otherwise <c>NULL</c>.</returns>
        [OperationContract]
        ServiceBinaryPropertyDefinition GetBinaryPropertyDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myPropertyName);

		/// <summary>
		/// Has this vertex type any binary property.
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>True if a binary property exists, otherwise false.</returns>
        [OperationContract]
        bool HasBinaryProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// Get all binary properties.
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>An enumerable of binary property definitions.</returns>
        [OperationContract]
        List<ServiceBinaryPropertyDefinition> GetBinaryPropertyDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		#endregion

		#region Edges

		#region Incoming

		/// <summary>
		/// Has this vertex type a certain incoming IncomingEdge?
		/// </summary>
		/// <returns>True or false</returns>
		[OperationContract]
		bool HasIncomingEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myEdgeName);


		/// <summary>
		/// Gets a certain incoming IncomingEdge definition
		/// </summary>
		/// <param name="myEdgeName">The name of the interesting incoming IncomingEdge</param>
		/// <returns>An incoming IncomingEdge definition</returns>
		[OperationContract]
		ServiceIncomingEdgeDefinition GetIncomingEdgeDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myEdgeName);

		/// <summary>
		/// Has this vertex type any visible incoming edges?
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>True or false</returns>
		[OperationContract]
		bool HasIncomingEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// Get all incoming edges
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>An enumerable of incoming IncomingEdge attributes</returns>
		[OperationContract]
		List<ServiceIncomingEdgeDefinition> GetIncomingEdgeDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);


		#endregion

		#region Outgoing

		/// <summary>
		/// Has this vertex type a certain outgoing IncomingEdge?
		/// </summary>
		/// <param name="myEdgeName">The name of the interesting outgoing IncomingEdge</param>
		/// <returns>True or false</returns>
		[OperationContract(Name="HasOutgoingEdgeByVertexType")]
		bool HasOutgoingEdge(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myEdgeName);

		/// <summary>
		/// Gets a certain outgoing IncomingEdge definition
		/// </summary>
		/// <param name="myEdgeName">The name of the interesting outgoing IncomingEdge</param>
		/// <returns>An outgoing IncomingEdge definition</returns>
		[OperationContract(Name = "GetOutgoingEdgeDefinitionByVertexType")]
		ServiceOutgoingEdgeDefinition GetOutgoingEdgeDefinition(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, String myEdgeName);

		/// <summary>
		/// Has this vertex type any outgoing edges?
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>True or false</returns>
		[OperationContract(Name="HasOutgoingEdgesByVertexType")]
		bool HasOutgoingEdges(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// Get all outgoing edges
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>An enumerable of outgoing IncomingEdge attributes</returns>
		[OperationContract(Name = "GetOutgoingEdgeDefinitionsByVertexType")]
		List<ServiceOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		#endregion

		#endregion

		#region Uniques

		/// <summary>
		/// 
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns></returns>
		[OperationContract]
		bool HasUniqueDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// A set of uniqueness definitions.
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns>An enumerable of uniqueness definitions. Never <c>NULL</c>.</returns>
		[OperationContract]
		List<ServiceUniqueDefinition> GetUniqueDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		#endregion

		#region Indices

		/// <summary>
		/// Returns true if there any index definition an this vertex type.
		/// </summary>
		/// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
		/// <returns></returns>
		[OperationContract]
		bool HasIndexDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		/// <summary>
		/// A set of index definitions.
		/// </summary>
		/// <returns>An enumerable of index definitions. Never <c>NULL</c>.</returns>
		[OperationContract]
		List<ServiceIndexDefinition> GetIndexDefinitions(SecurityToken mySecurityToken, Int64 myTransToken, ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions);

		#endregion
	}
}
