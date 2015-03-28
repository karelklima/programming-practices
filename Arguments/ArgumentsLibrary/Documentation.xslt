﻿<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!--                Created by Rick Bielawski - published 2011-02-18                   -->
  <!--                             last updated 2011-06-05                               -->
  <!--  To display an XML documentation file generated by Visual Studio with a browser   -->
  <!--  add the line below immediately after the ?xml version line in your XML document  -->
  <!--  then open the XML file with your browser. Obviously it must point to THIS file.  -->
  <!--        <?xml-stylesheet href="visual-studio-xml-doc.xsl" type="text/xsl"?>        -->
  <!--  Or, use an XSLT generator with this code on your XML document to create html     -->

  <!-- ======================================================================
       Here we declare ancilary routines used to extract data
       ====================================================================== -->
  <!--  Replace a given string with another.  Basically this is only needed because long
        parameter lists don't wrap at a comma in HTML without a space being inserted.  -->
  <xsl:template name="tReplaceSubString">
    <xsl:param name="pText" />
    <xsl:param name="pLookFor" select="','" />
    <xsl:param name="pNewValue" select="' ,'" />
    <xsl:choose>
      <xsl:when test="contains($pText,$pLookFor)">
        <xsl:value-of select="substring-before($pText,$pLookFor)" />
        <xsl:value-of select="$pNewValue" />
        <xsl:call-template name="tReplaceSubString">
          <xsl:with-param name="pText"     select="substring-after($pText,$pLookFor)" />
          <xsl:with-param name="pLookFor"  select="$pLookFor" />
          <xsl:with-param name="pNewValue" select="$pNewValue" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$pText" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!--  Counting the number of levels in a member's name allows me to tell
        where the name of the entity being documented starts (after the last one).  -->
  <xsl:template name="tCountOccurances">
    <xsl:param name="pText" />
    <xsl:param name="pLookFor" select="'.'" />
    <xsl:param name="pCount" select="0" />
    <xsl:choose>
      <xsl:when test="contains($pText,$pLookFor)">
        <xsl:call-template name="tCountOccurances">
          <xsl:with-param name="pText"    select="substring-after($pText,$pLookFor)" />
          <xsl:with-param name="pLookFor" select="$pLookFor" />
          <xsl:with-param name="pCount"   select="$pCount + 1" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$pCount" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!--  Replaces the pCount instance of pLookFor with pSeparator.  Used to split a string.
        pCount=0 doesn't look for the pLookFor.  It just puts pSeperator at the beginning.
        pCount=1 splits at the 1st occurance, 2 splits at the 2nd occurance....
        Splitting on a count greater than the number of occurances puts pSeperator at the end.  -->
  <xsl:template name="tSplitAtInstance">
    <xsl:param name="pText" />
    <xsl:param name="pCount" select="0" />
    <xsl:param name="pLookFor" select="'.'" />
    <xsl:param name="pSeparator" select="'|'" />
    <xsl:param name="pStartEmpty" />
    <xsl:choose>
      <xsl:when test="$pCount &gt; 0 and contains($pText,$pLookFor)">
        <xsl:variable name="temp">
          <xsl:if test="$pStartEmpty = ''">
            <xsl:value-of select="substring-before($pText,$pLookFor)" />
          </xsl:if>
          <xsl:if test="$pStartEmpty != ''">
            <xsl:value-of select="concat($pStartEmpty,$pLookFor,substring-before($pText,$pLookFor))" />
          </xsl:if>
        </xsl:variable>
        <xsl:call-template name="tSplitAtInstance">
          <xsl:with-param name="pText"       select="substring-after($pText,$pLookFor)" />
          <xsl:with-param name="pCount"      select="$pCount - 1" />
          <xsl:with-param name="pLookFor"    select="$pLookFor" />
          <xsl:with-param name="pSeparator"  select="$pSeparator" />
          <xsl:with-param name="pStartEmpty" select="$temp" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="concat($pStartEmpty,$pSeparator,$pText)" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- ======================================================================
       These format individual parts of the document.
       ====================================================================== -->

  <!--  This seems to be the only thing that can be done generically -->
  <xsl:template name="tElementName">
    <xsl:param name="pText" />
    <span class="ElementName">
      <xsl:value-of select="$pText" />:
    </span>
  </xsl:template>

  <!-- ======================================================================
 
       This is where actual transformation starts
 
       ====================================================================== -->

  <!--  Set a var with the name of the assembly for the documented project. -->
  <xsl:variable name="vAssembly">
    <xsl:value-of select="doc/assembly/name" />
  </xsl:variable>

  <!--  This creates the HTML document within which the XML content is formatted. -->
  <xsl:template match="doc">
    <HTML>
      <HEAD>
        <style type="text/css">
          h1 {font-weight:bold; text-decoration:underline; text-align:center}
          pre {margin-top:0; margin-bottom:0}
          table {margin-left:5mm; margin-top:0; margin-bottom:0}
          tr#FirstRow {font-size:1.2em; text-decoration:underline}
          td {padding-left:2em}

          .AttributeName {font-weight:bold; padding-right:1em; color:blue; font-family:"Monospace"}
          .ClassName {color:red}
          .CodeItem {font-family:"Monospace"}
          .ElementName {padding-right:5px; text-transform:capitalize; font-weight:bold; font-style:italic}
          .EntityName {margin-left:2em; font-weight:bold; font-size:1.2em; font-family:"Monospace"}
          .MemberTypeName {color:blue}

          .summary {margin-top:0.3em; margin-bottom:0.3em}
          .returns {margin-top:0.3em; margin-bottom:0.3em}
          .exception {margin-top:0.3em; margin-bottom:0.3em}
          .remarks {margin-top:0.3em; margin-bottom:0.3em}
        </style>
        <TITLE>
          <xsl:value-of select="$vAssembly" /> documentation
        </TITLE>
      </HEAD>
      <BODY>
        <h1>
          <xsl:value-of select="$vAssembly" /> documentation
        </h1>
        <p />
        <xsl:apply-templates select="members/member" />
      </BODY>
    </HTML>
  </xsl:template>

  <!--  This template first extracts information about the Member being formatted   -->
  <!--  Then formats the individual doc sections: summary, param, return...   -->
  <xsl:template match="member">

    <!-- I decided to use the <para> tag to flag the entity as public.  This way I can
         document internal stuff for intellisense but not for users.  It works well
         since intellisense ignores <para> formatting anyway.  -->
    <xsl:if test="count(summary/para) > 0">

      <!--  The whole name has the type, namespace, class name, and member name.   -->
      <xsl:variable name="vWholeName">
        <xsl:value-of select="@name" />
      </xsl:variable>

      <!--  The type of member being documented is encoded in the first character   -->
      <xsl:variable name="vMemberType">
        <xsl:value-of select="substring-before($vWholeName,':')" />
      </xsl:variable>
      <xsl:variable name="vMemberTypeName">
        <xsl:choose>
          <xsl:when test="$vMemberType = 'E'">Event</xsl:when>
          <xsl:when test="$vMemberType = 'F'">Field</xsl:when>
          <xsl:when test="$vMemberType = 'M'">Method</xsl:when>
          <xsl:when test="$vMemberType = 'N'">NameSpace</xsl:when>
          <xsl:when test="$vMemberType = 'P'">Property</xsl:when>
          <xsl:when test="$vMemberType = 'T'">Type</xsl:when>
          <xsl:otherwise>Unknown</xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <!--  We need the number of name parts to get the last part (the name of the entity).
            Unfortunately there might be periods in parameter data types so we must count
            only those that come before any open paren.  -->
      <xsl:variable name="vNumPeriods">
        <xsl:variable name="temp">
          <xsl:choose>
            <xsl:when test="contains($vWholeName,'(')">
              <xsl:value-of select="substring-before($vWholeName,'(')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$vWholeName" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:call-template name="tCountOccurances">
          <xsl:with-param name="pText" select="$temp" />
          <xsl:with-param name="pLookFor" select="'.'" />
        </xsl:call-template>
      </xsl:variable>

      <!--  Split the full name between entity name (and any args) and the path (ns,class...)  -->
      <xsl:variable name="vSplitName">
        <xsl:call-template name="tSplitAtInstance">
          <xsl:with-param name="pText"      select="$vWholeName" />
          <xsl:with-param name="pCount"     select="$vNumPeriods" />
          <xsl:with-param name="pLookFor"   select="'.'" />
          <xsl:with-param name="pSeparator" select="'|'" />
        </xsl:call-template>
      </xsl:variable>

      <!--  Get the entity name (and any args).
            Add space to commas so browsers can wrap them if needed  -->
      <xsl:variable name="vEntityName">
        <xsl:call-template name="tReplaceSubString">
          <xsl:with-param name="pText"     select="substring-after($vSplitName,'|')" />
          <xsl:with-param name="pLookFor"  select="','" />
          <xsl:with-param name="pNewValue" select="', '" />
        </xsl:call-template>
      </xsl:variable>

      <!--  We'll assume the assembly name is also the prime Namespace name and omit it.
            The 'class' name might also include any sub-namespace names.  -->
      <xsl:variable name="vClass">
        <xsl:value-of select="substring-after(substring-before($vSplitName,'|'),'.')" />
      </xsl:variable>

      <!--  Variables are populated.  This formats member data on the page.  -->
      <xsl:if test="not($vClass='')">
        <span class="ClassName">
          <xsl:value-of select="$vClass" /> -
        </span>
      </xsl:if>
      <span class="MemberTypeName">
        <xsl:value-of select="$vMemberTypeName" />
      </span>:
      <div class="EntityName">
        <xsl:value-of select="$vEntityName" />
        <xsl:if test="$vMemberType = 'M' and not(contains($vWholeName,'('))">()</xsl:if>
      </div>
      <!-- And now, report all the sub-elements -->
      <xsl:apply-templates select="summary" />
      <xsl:apply-templates select="param" />
      <xsl:apply-templates select="returns" />
      <xsl:apply-templates select="interface" />
      <xsl:apply-templates select="exception" />
      <xsl:apply-templates select="example" />
      <xsl:apply-templates select="remarks" />
      <hr />
    </xsl:if>
  </xsl:template>

  <!-- These format the primary documentation elements -->

  <xsl:template match="summary">
    <div class="summary">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      <xsl:apply-templates select="node()" />
    </div>
  </xsl:template>
  <xsl:template match="param">
    <div class="param">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      <span class="AttributeName">
        <xsl:value-of select="@name" />
      </span>
      <xsl:apply-templates select="node()" />
    </div>
  </xsl:template>
  <xsl:template match="returns">
    <div class="returns">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      <xsl:apply-templates select="node()" />
    </div>
  </xsl:template>
  <xsl:template match="interface">
    <div class="interface">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      <span class="AttributeName">
        <xsl:value-of select="@name" />
      </span>
      <xsl:apply-templates select="node()" />
    </div>
  </xsl:template>
  <xsl:template match="exception">
    <div class="exception">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      (<xsl:value-of select="substring-after(@cref,'.')" />)
      <xsl:apply-templates select="node()" />
    </div>
  </xsl:template>
  <xsl:template match="example">
    <div class="example">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      <pre>
        <xsl:value-of select="." />
      </pre>
    </div>
  </xsl:template>
  <xsl:template match="remarks">
    <div class="remarks">
      <xsl:call-template name="tElementName">
        <xsl:with-param name="pText" select="name()" />
      </xsl:call-template>
      <xsl:apply-templates select="node()" />
    </div>
  </xsl:template>

  <!-- Document elements can contain custom formatting elements.  -->

  <xsl:template match="para">
    <xsl:if test="not(.='')">
      <xsl:apply-templates select="node()" />
      <br />
    </xsl:if>
  </xsl:template>

  <xsl:template match="c">
    <span class="CodeItem">
      <xsl:value-of select="." />
    </span>
  </xsl:template>

  <xsl:template match="list">
    <table>
      <tr id="FirstRow">
        <td>Item</td>
        <td>Description</td>
      </tr>
      <xsl:apply-templates select="node()" />
    </table>
  </xsl:template>
  <xsl:template match="item">
    <tr>
      <xsl:apply-templates select="node()" />
    </tr>
  </xsl:template>
  <xsl:template match="term">
    <td>
      <xsl:value-of select="." />
    </td>
  </xsl:template>
  <xsl:template match="description">
    <td>
      <xsl:value-of select="." />
    </td>
  </xsl:template>

</xsl:stylesheet>