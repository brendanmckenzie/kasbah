import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import Node from './Node'

class NodeTree extends React.Component {
  static propTypes = {
    parent: PropTypes.string,
    parentAlias: PropTypes.string,
    content: PropTypes.object.isRequired
  }

  static contextTypes = {
    tree: PropTypes.array.isRequired,
    onCreateNode: PropTypes.func.isRequired,
    readOnly: PropTypes.bool
  }

  handleCreateNode = () => this.context.onCreateNode(this.props.parent)

  render() {
    if (!this.props.content.tree.loaded) { return null }

    const { parent, parentAlias } = this.props
    const { tree: { nodes }, readOnly } = this.props.content

    return (
      <ul className='content-tree'>
        {nodes.filter(ent => ent.parent === parent).map(ent => (
          <Node key={ent.id} node={ent} />
        ))}
        {!readOnly && (<li>
          <button
            className='button is-small add-node'
            onClick={this.handleCreateNode}>{parentAlias ? (<span>New node under <strong>{parentAlias}</strong></span>) : 'New node'}</button>
        </li>)}
      </ul>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content
})

export default connect(mapStateToProps)(NodeTree)
