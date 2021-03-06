/*
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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;

#endregion

namespace sones.GraphDS.Services.RESTService
{
    public static class GraphDSREST_Constants
    {
        public static ContentType _TEXT_UTF8 = new ContentType(MediaTypeNames.Text.Plain) { CharSet = "UTF-8" };
        public static ContentType _HTML_UTF8 = new ContentType("text/html") { CharSet = "UTF-8" };

        public static ContentType _CSS = new ContentType("text/css");
        public static ContentType _GIF = new ContentType("image/gif");
        public static ContentType _ICO = new ContentType("image/ico");
        public static ContentType _PNG = new ContentType("image/png");
        public static ContentType _JPG = new ContentType("image/jpg");

        public static ContentType _OCTET = new ContentType("application/octet-stream");
    
    }
}
