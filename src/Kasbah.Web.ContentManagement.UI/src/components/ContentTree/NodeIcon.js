import React from 'react'
import PropTypes from 'prop-types'

const NodeIcon = ({ content, node }) => {
  const { types } = content
  const type = types.list.find(ent => ent.alias.split(',')[0] === node.type.split(',')[0])

  if (type) {
    if (type.icon) {
      const attrs = {
        className: `fa fa-${type.icon}`,
        style: {
          color: type.iconColour
        }
      }
      return <i {...attrs} />
    }
  }

  return <i className='fa fa-file' />
}

NodeIcon.propTypes = {
  content: PropTypes.object.isRequired,
  node: PropTypes.object.isRequired
}

export default NodeIcon
