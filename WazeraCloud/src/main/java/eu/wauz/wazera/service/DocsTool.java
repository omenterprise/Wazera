package eu.wauz.wazera.service;

import org.apache.commons.lang3.StringUtils;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;

public class DocsTool {

	public void checkForValidFileName(String fileName) throws Exception {
		if(StringUtils.containsAny(fileName, new char[] {'|', '/', '\\', ':', '*', '?', '"', '\'', '<', '>'})) {
			throw new Exception("The input contained invalid characters!");
		}
	}
	
	public String getUsername() {
		Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
		return authentication != null ? authentication.getName() : null;
	}

}