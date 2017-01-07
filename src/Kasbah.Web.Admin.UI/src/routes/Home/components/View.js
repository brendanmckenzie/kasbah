import React from 'react'
import { Link } from 'react-router'

export const View = () => (
  <div className='section'>
    <div className='container'>
      <div className='tile is-ancestor'>
        <div className='tile is-vertical is-8'>
          <div className='tile'>
            <div className='tile is-parent is-vertical'>
              <article className='tile is-child notification is-danger'>
                <p className='title'>Kasbah</p>
                <p className='subtitle'>v1.0</p>
              </article>
              <Link to='/security' className='tile is-child notification is-warning'>
                <p className='title'>Security</p>
                <p className='subtitle'>Manage user access and roles</p>
              </Link>
            </div>
            <div className='tile is-parent'>
              <Link to='/analytics' className='tile is-child notification is-info'>
                <p className='title'>Analytics</p>
                <p className='subtitle'>Some sort of analytics here</p>
              </Link>
            </div>
          </div>
          <div className='tile is-parent'>
            <article className='tile is-child notification'>
              <p className='title'>System information</p>
              <p className='subtitle'>Aligned with the right tile</p>
              <div className='content'>
                <p>Server statistics, requests/minute, uptime, total # of requests...</p>
              </div>
            </article>
          </div>
        </div>
        <div className='tile is-parent'>
          <Link to='/content' className='tile is-child notification is-success'>
            <div className='content'>
              <p className='title'>Content</p>
              <p className='subtitle'>Maybe put the content tree here?</p>
              <div className='content'>
                <p>Or maybe a small listing of recently updated content items?</p>
              </div>
            </div>
          </Link>
        </div>
      </div>
    </div>
  </div>
)

export default View
