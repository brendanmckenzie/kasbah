import React from 'react'
import { Link } from 'react-router'

export const Navigation = () => (
  <div className='menu'>
    <span className='menu-label'>Sections</span>
    <ul className='menu-list'>
      <li className='menu-item'>
        <Link activeClassName='is-active' to='/system/sites'>Sites</Link>
      </li>
    </ul>
  </div>
)

export default Navigation
