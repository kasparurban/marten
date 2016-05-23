﻿using System;
using System.Linq;
using Baseline;
using Marten.Schema;
using Marten.Testing.Documents;
using Marten.Testing.Schema.Hierarchies;
using Shouldly;
using Xunit;

namespace Marten.Testing
{
    public class DocumentMappingTests
    {
        public class FieldId
        {
            public string id;
        }

        public abstract class AbstractDoc
        {
            public int id;
        }

        public interface IDoc
        {
            string id { get; set; }
        }

        [Fact]
        public void concrete_type_with_subclasses_is_hierarchy()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.AddSubClass(typeof(SuperUser));

            mapping.IsHierarchy().ShouldBeTrue();
        }

        [Fact]
        public void default_alias_for_a_type_that_is_not_nested()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias.ShouldBe("user");
        }

        [Fact]
        public void default_table_name()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Table.Name.ShouldBe("mt_doc_user");
        }

        [Fact]
        public void default_table_name_with_different_shema()
        {
            var mapping = DocumentMapping.For<User>("other");
            mapping.Table.QualifiedName.ShouldBe("other.mt_doc_user");
        }

        [Fact]
        public void default_table_name_with_schema()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Table.QualifiedName.ShouldBe("public.mt_doc_user");
        }

        [Fact]
        public void default_upsert_name()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.UpsertFunction.Name.ShouldBe("mt_upsert_user");
        }

        [Fact]
        public void default_upsert_name_with_different_schema()
        {
            var mapping = DocumentMapping.For<User>("other");
            mapping.UpsertFunction.QualifiedName.ShouldBe("other.mt_upsert_user");
        }

        [Fact]
        public void default_upsert_name_with_schema()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.UpsertFunction.QualifiedName.ShouldBe("public.mt_upsert_user");
        }

        [Fact]
        public void get_the_sql_locator_for_the_Id_member()
        {
            DocumentMapping.For<User>().FieldFor("Id")
                .SqlLocator.ShouldBe("d.id");

            DocumentMapping.For<FieldId>().FieldFor("id")
                .SqlLocator.ShouldBe("d.id");
        }

        [Fact]
        public void is_hierarchy__is_false_for_concrete_type_with_no_subclasses()
        {
            DocumentMapping.For<User>().IsHierarchy().ShouldBeFalse();
        }

        [Fact]
        public void is_hierarchy_always_true_for_abstract_type()
        {
            DocumentMapping.For<AbstractDoc>()
                .IsHierarchy().ShouldBeTrue();
        }

        [Fact]
        public void is_hierarchy_always_true_for_interface()
        {
            DocumentMapping.For<IDoc>().IsHierarchy()
                .ShouldBeTrue();
        }

        [Fact]
        public void optimistic_versioning_is_turned_off_by_default()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.UseOptimisticConcurrency.ShouldBeFalse();
        }

        [Fact]
        public void override_the_alias()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias = "users";

            mapping.Table.Name.ShouldBe("mt_doc_users");
            mapping.UpsertFunction.Name.ShouldBe("mt_upsert_users");
        }

        [Fact]
        public void override_the_alias_converts_alias_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias = "Users";

            mapping.Alias.ShouldBe("users");
        }

        [Fact]
        public void override_the_alias_converts_table_name_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias = "Users";

            mapping.Table.Name.ShouldBe("mt_doc_users");
        }

        [Fact]
        public void override_the_alias_converts_tablename_with_different_schema_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>("OTHER");
            mapping.Alias = "Users";

            mapping.Table.QualifiedName.ShouldBe("other.mt_doc_users");
        }

        [Fact]
        public void override_the_alias_converts_tablename_with_schema_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias = "Users";

            mapping.Table.QualifiedName.ShouldBe("public.mt_doc_users");
        }

        [Fact]
        public void override_the_alias_converts_upsertname_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias = "Users";

            mapping.UpsertFunction.Name.ShouldBe("mt_upsert_users");
        }

        [Fact]
        public void override_the_alias_converts_upsertname_with_different_schema_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>("OTHER");
            mapping.Alias = "Users";

            mapping.UpsertFunction.QualifiedName.ShouldBe("other.mt_upsert_users");
        }

        [Fact]
        public void override_the_alias_converts_upsertname_with_schema_to_lowercase()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.Alias = "Users";

            mapping.UpsertFunction.QualifiedName.ShouldBe("public.mt_upsert_users");
        }

        [Fact]
        public void select_fields_for_non_hierarchy_mapping()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.SelectFields().ShouldHaveTheSameElementsAs("data", "id");
        }

        [Fact]
        public void select_fields_with_subclasses()
        {
            var mapping = DocumentMapping.For<Squad>();
            mapping.AddSubClass(typeof(BaseballTeam));

            mapping.SelectFields().ShouldHaveTheSameElementsAs("data", "id", DocumentMapping.DocumentTypeColumn);
        }

        [Fact]
        public void select_fields_without_subclasses()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.SelectFields().ShouldHaveTheSameElementsAs("data", "id");
        }

        [Fact]
        public void to_table_columns_with_duplicated_fields()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.DuplicateField("FirstName");
            mapping.SchemaObjects.As<DocumentSchemaObjects>().StorageTable().Columns.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("id", "data", DocumentMapping.LastModifiedColumn,
                    DocumentMapping.VersionColumn, DocumentMapping.DotNetTypeColumn, "first_name");
        }

        [Fact]
        public void to_table_columns_with_subclasses()
        {
            var mapping = DocumentMapping.For<Squad>();
            mapping.AddSubClass(typeof(BaseballTeam));

            var table = mapping.SchemaObjects.As<DocumentSchemaObjects>().StorageTable();

            var typeColumn = table.Columns.Last();
            typeColumn.Name.ShouldBe(DocumentMapping.DocumentTypeColumn);
            typeColumn.Type.ShouldBe("varchar");
        }

        [Fact]
        public void to_table_without_subclasses_and_no_duplicated_fields()
        {
            var mapping = DocumentMapping.For<IntDoc>();
            mapping.SchemaObjects.As<DocumentSchemaObjects>().StorageTable().Columns.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("id", "data", DocumentMapping.LastModifiedColumn,
                    DocumentMapping.VersionColumn, DocumentMapping.DotNetTypeColumn);
        }

        [Fact]
        public void to_upsert_baseline()
        {
            var mapping = DocumentMapping.For<Squad>();
            var function = new UpsertFunction(mapping);

            function.Arguments.Select(x => x.Column)
                .ShouldHaveTheSameElementsAs("id", "data", DocumentMapping.VersionColumn,
                    DocumentMapping.DotNetTypeColumn);
        }

        [Fact]
        public void to_upsert_with_duplicated_fields()
        {
            var mapping = DocumentMapping.For<User>();
            mapping.DuplicateField("FirstName");
            mapping.DuplicateField("LastName");

            var function = new UpsertFunction(mapping);

            function.Arguments.Select(x => x.Column)
                .ShouldHaveTheSameElementsAs("id", "data", "first_name", "last_name", DocumentMapping.VersionColumn,
                    DocumentMapping.DotNetTypeColumn);
        }

        [Fact]
        public void to_upsert_with_subclasses()
        {
            var mapping = DocumentMapping.For<Squad>();
            mapping.AddSubClass(typeof(BaseballTeam));

            var function = new UpsertFunction(mapping);

            function.Arguments.Select(x => x.Column)
                .ShouldHaveTheSameElementsAs("id", "data", DocumentMapping.VersionColumn,
                    DocumentMapping.DotNetTypeColumn, DocumentMapping.DocumentTypeColumn);
        }

        [Fact]
        public void doc_type_with_use_optimistic_concurrency_attribute()
        {
            DocumentMapping.For<VersionedDoc>()
                .UseOptimisticConcurrency.ShouldBeTrue();
        }

        [UseOptimisticConcurrency]
        public class VersionedDoc
        {
            public Guid Id { get; set; } = Guid.NewGuid();
        }
    }
}