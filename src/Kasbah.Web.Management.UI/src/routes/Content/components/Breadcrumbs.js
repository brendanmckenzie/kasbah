import React from 'react'
import PropTypes from 'prop-types'
import { NavLink } from 'react-router-dom'

const Breadcrumb = ({ content, id }) => {
  const detail = content.detail[id]

  if (!detail) {
    return null
  }

  const { taxonomy } = detail.node

  return (<div className='breadcrumb'>
    <ul>
      {taxonomy.aliases.map((ent, index) =>
        (<li key={index} className={taxonomy.ids[index] === id ? 'is-active' : ''}><NavLink to={`/content/${taxonomy.ids[index]}`}>{ent}</NavLink></li>)
      )}
    </ul>
  </div>)
}

Breadcrumb.propTypes = {
  id: PropTypes.string.isRequired,
  content: PropTypes.object.isRequired
}

export default Breadcrumb
