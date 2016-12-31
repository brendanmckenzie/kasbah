import React from 'react'
import { IndexLink, Link } from 'react-router'

export const Header = () => (
  <header className='nav'>
    <div className='container'>
      <ul className='nav-left'>
        <li className='nav-item'><IndexLink to='/'><strong>KASBAH</strong></IndexLink></li>
      </ul>

      <ul className='nav-right'>
        <li className='nav-item'>
          <Link activeClassName='is-active' to='/content'>Content</Link>
        </li>
        <li className='nav-item'>
          <Link activeClassName='is-active' to='/media'>Media</Link>
        </li>
        <li className='nav-item'>
          <Link activeClassName='is-active' to='/analytics'>Analytics</Link>
        </li>
        <li className='nav-item'>
          <Link activeClassName='is-active' to='/security'>Security</Link>
        </li>
      </ul>
    </div>
  </header>
)

export default Header
