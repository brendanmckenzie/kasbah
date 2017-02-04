import React from 'react'
import { Link } from 'react-router'
import Content from './Content'

export const View = (props) => (
  <div className='section'>
    <div className='container'>
      <div className='tile is-ancestor'>
        <div className='tile is-vertical is-8'>
          <div className='tile'>
            <div className='tile is-parent is-vertical'>
              <article className='tile is-child notification is-danger'>
                <p className='heading'>Content authoring and marketing platform</p>
                <p className='title'>Kasbah</p>
                <p className='subtitle'>v1.0</p>
              </article>
              <Link to='/security' className='tile is-child notification is-warning'>
                <p className='title'>Security</p>
                <p className='subtitle'>Manage user access and roles</p>
              </Link>
            </div>
            <div className='tile is-parent'>
              <div className='tile is-child notification is-info'>
                <h2 className='title'><Link to='/analytics'>Analytics</Link></h2>
                <p className='subtitle'>Some sort of analytics here</p>
              </div>
            </div>
          </div>
          <div className='tile is-parent'>
            <div className='tile is-light is-child notification'>
              <h2 className='title'><Link to='/system'>System information</Link></h2>
              <p className='subtitle'>Aligned with the right tile</p>
              <div className='content'>
                <p>Server statistics, requests/minute, uptime, total # of requests...</p>
              </div>
            </div>
          </div>
        </div>
        <div className='tile is-parent'>
          <Content {...props} />
        </div>
      </div>
    </div>
  </div>
)

View.propTypes = {
  listLatestUpdatesRequest: React.PropTypes.func.isRequired,
  listLatestUpdates: React.PropTypes.object.isRequired
}

export default View
