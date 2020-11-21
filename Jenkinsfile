pipeline {
	agent any

	environment {
		SCANNER_HOME =  tool 'SonarQubeScanner'
    }

	stages {
		
		stage('OWASP DependencyCheck') {
			steps {
				dependencyCheck additionalArguments: '--format HTML --format XML', odcInstallation: 'Default'
			}

			post {
				success {
					dependencyCheckPublisher pattern: 'dependency-check-report.xml'
				}
			}
		}

		stage('SonarQube Analysis') {
			steps {
				withSonarQubeEnv('Default') {
					sh "$SCANNER_HOME/bin/sonar-scanner"
				}
			}
		}

	}

}
