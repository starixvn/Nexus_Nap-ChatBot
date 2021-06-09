@Library("JenkinsPipelineLibrary") _

pipeline{
  agent any

  options{
    buildDiscarder(logRotator(numToKeepStr: '5'))
    disableConcurrentBuilds()
  }

  triggers{
    pollSCM("H/15 * * * *")
  }

  stages{
    stage('Build'){
      steps {
        script{
          pipelineLib.beginSonarQubeForMsBuild("NAPCHATBOT", "Nexus / NAP ChatBot", "/d:sonar.cs.opencover.reportsPaths=\"${WORKSPACE}\\CoverageReports\\*.xml\" /d:sonar.cs.vstest.reportsPaths=\"${WORKSPACE}\\TestResults\\*.trx\" /d:sonar.javascript.lcov.reportPaths=\"${WORKSPACE}\\js-lcov\\lcov.info\" /d:sonar.testExecutionReportPaths=\"${WORKSPACE}\\js-test-report.xml\" /d:sonar.tests=__tests__ /d:sonar.test.inclusions=**/*.test.js")
          pipelineLib.msBuild16("Fanex.NCB.sln")
        }
      }
    }
	
	stage("Test") {
	  parallel{
	    stage("C# Unit Test"){
		  steps{
            script{
              pipelineLib.xUnitForNet5()
            }
          }
		}
		
		stage("JS Unit Test"){
		  steps{
            script{
		      pipelineLib.jsJest("Fanex.NCB.Main\\ClientApp")
            }
          }
		}
	  }
    }

    stage("SonarQube Analysis"){
      steps{
        script{
          pipelineLib.endSonarQubeForMsBuild()
        }
      }

      post{
        always{
          script{
            if(manager.logContains(".*Quality gate status.*")){
              pipelineLib.generateSonarQubeReport("NAPCHATBOT")
            }
          }
          archiveArtifacts "*.xml,*.email"
          step([$class: 'ACIPluginPublisher', name: '*.xml', shownOnProjectPage: false])
          mstest failOnError: false
        }
      }
    }
  }

  post{
    unsuccessful{
      emailext body: '$DEFAULT_CONTENT', subject: '$DEFAULT_SUBJECT', to: 'quinn.vo@starixvn.com,tim.trinh@starixvn.com,oliver.dang@starixvn.com'
    }
  }
}