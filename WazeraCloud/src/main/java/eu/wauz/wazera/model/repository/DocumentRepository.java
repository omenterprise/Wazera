package eu.wauz.wazera.model.repository;

import java.util.List;

import org.springframework.data.repository.CrudRepository;

import eu.wauz.wazera.model.entity.Document;

public interface DocumentRepository extends CrudRepository<Document, Integer> {

	List<Document> findAll();

	List<Document> findByFolderIdOrderByName(Integer folderId);

	List<Document> findByFolderIdOrderBySortOrder(Integer folderId);

	Document findByAbsoluteFilename(String absoluteFilename);

	Document findByNameAndFolderId(String name, Integer id);

}
