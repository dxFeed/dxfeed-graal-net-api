// <copyright file="HelpArticles.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DxFeed.Graal.Net.Tools.Help;

public class HelpArticles
{
    private readonly IEnumerable<XmlNode> _articles;

    public HelpArticles(string xml)
    {
        var helpDoc = new XmlDocument();
        helpDoc.LoadXml(xml);

        // Possible null reference argument for parameter. False positive.
#pragma warning disable CS8604
        _articles =
            from article in helpDoc.SelectSingleNode("//articles")?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>()
            where article.Attributes != null
            from attribute in article.Attributes.Cast<XmlAttribute>()
            where attribute.Name.Equals("name")
            select article;
#pragma warning restore CS8604

        ListOfAllArticles =
            from article in _articles
            select article.Attributes?["name"]?.Value;
    }

    public IEnumerable<string> ListOfAllArticles { get; }

    public bool ContainsArticle(string name) =>
        FindArticle(name) != null;

    public string? FindArticle(string name) =>
        _articles.FirstOrDefault(article => name.Equals(
            article.Attributes?["name"]?.Value,
            StringComparison.OrdinalIgnoreCase))?.InnerText;
}
