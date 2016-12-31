import React from 'react'
import Information from './Information'
import Analytics from './Analytics'
import ContentEditor from './ContentEditor'

export class View extends React.Component {
  constructor() {
    super()
    this.state = {
      sidebarSection: 'information'
    }

    this.setSidebarInformation = this.setSidebarInformation.bind(this)
    this.setSidebarAnalytics = this.setSidebarAnalytics.bind(this)
  }

  setSidebarInformation() {
    this.setState({
      sidebarSection: 'information'
    })
  }

  setSidebarAnalytics() {
    this.setState({
      sidebarSection: 'analytics'
    })
  }

  render() {
    return (
      <div>
        <h2 className='subtitle'>Website</h2>
        <div className='columns'>
          <div className='column'>
            <ContentEditor />
          </div>
          <div className='column is-3'>
            <nav className='panel'>
              <p className='panel-tabs'>
                <a className={this.state.sidebarSection === 'information' ? 'is-active' : null} onClick={this.setSidebarInformation}>Statistics</a>
                <a className={this.state.sidebarSection === 'analytics' ? 'is-active' : null} onClick={this.setSidebarAnalytics}>Analytics</a>
              </p>
              <div className='panel-block'>
                {this.state.sidebarSection === 'information' ? <Information /> : <Analytics />}
              </div>
            </nav>
          </div>
        </div>
      </div>
    )
  }
}

export default View
