import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import Node from './Node'

const NodeTree = ({ parent, content: { tree: { nodes } } }) => (
  <ul className='content-tree'>
    {nodes.filter(ent => ent.parent === parent).map(ent => <Node key={ent.id} node={ent} />)}
  </ul>
)

NodeTree.propTypes = {
  parent: PropTypes.string,
  content: PropTypes.object.isRequired
}

const mapStateToProps = (state) => ({
  content: state.content
})

export default connect(mapStateToProps)(NodeTree)
