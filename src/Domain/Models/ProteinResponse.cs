using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Models;

public class ProteinResponse
{
    public List<Result> Results { get; set; }
}

public class Result
{
    public string EntryType { get; set; }
    public string PrimaryAccession { get; set; }
    public string UniProtkbId { get; set; }
    public EntryAudit EntryAudit { get; set; }
    public double AnnotationScore { get; set; }
    public Organism Organism { get; set; }
    public string ProteinExistence { get; set; }
    public ProteinDescription ProteinDescription { get; set; }
    public List<Gene> Genes { get; set; }
    public List<Feature> Features { get; set; }
    public List<Keyword> Keywords { get; set; }
    public List<CrossReference> UniProtKBCrossReferences { get; set; }
    public Sequence Sequence { get; set; }
    public ExtraAttributes ExtraAttributes { get; set; }
}

public class EntryAudit
{
    public string FirstPublicDate { get; set; }
    public string LastAnnotationUpdateDate { get; set; }
    public string LastSequenceUpdateDate { get; set; }
    public int EntryVersion { get; set; }
    public int SequenceVersion { get; set; }
}

public class Organism
{
    public string ScientificName { get; set; }
    public int TaxonId { get; set; }
    public List<Evidence> Evidences { get; set; }
    public List<string> Lineage { get; set; }
}

public class Evidence
{
    public string EvidenceCode { get; set; }
    public string Source { get; set; }
    public string Id { get; set; }
}

public class ProteinDescription
{
    public RecommendedName RecommendedName { get; set; }
}

public class RecommendedName
{
    public FullName FullName { get; set; }
}

public class FullName
{
    public List<Evidence> Evidences { get; set; }
    public string Value { get; set; }
}

public class Gene
{
    public List<OrderedLocusName> OrderedLocusNames { get; set; }
}

public class OrderedLocusName
{
    public List<Evidence> Evidences { get; set; }
    public string Value { get; set; }
}

public class Feature
{
    public string Type { get; set; }
    public string Description { get; set; }
    public List<Evidence> Evidences { get; set; }
    public Ligand Ligand { get; set; }
}





public class Ligand
{
    public string Name { get; set; }
    public string Id { get; set; }
}

public class Keyword
{
    public List<Evidence> Evidences { get; set; }
    public string Id { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
}

public class Reference
{
    public int ReferenceNumber { get; set; }
    public Citation Citation { get; set; }
    public List<string> ReferencePositions { get; set; }
    public List<ReferenceComment> ReferenceComments { get; set; }
    public List<Evidence> Evidences { get; set; }
}

public class Citation
{
    public string Id { get; set; }
    public string CitationType { get; set; }
    public List<string> Authors { get; set; }
    public List<CitationCrossReference> CitationCrossReferences { get; set; }
    public string Title { get; set; }
    public string PublicationDate { get; set; }
    public string Journal { get; set; }
    public string FirstPage { get; set; }
    public string LastPage { get; set; }
    public string Volume { get; set; }
}

public class CitationCrossReference
{
    public string Database { get; set; }
    public string Id { get; set; }
}

public class ReferenceComment
{
    public List<Evidence> Evidences { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
}

public class CrossReference
{
    public string Database { get; set; }
    public string Id { get; set; }
    public List<Property> Properties { get; set; }
}

public class Property
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class Sequence
{
    public string Value { get; set; }
    public int Length { get; set; }
    public double MolWeight { get; set; }
    public string Crc64 { get; set; }
    public string Md5 { get; set; }
}

public class ExtraAttributes
{
    public CountByFeatureType CountByFeatureType { get; set; }
    public string UniParcId { get; set; }
}

public class CountByFeatureType
{
    public int Transmembrane { get; set; }
    public int BindingSite { get; set; }
}
