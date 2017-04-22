import React from 'react'
import ContentTree from './ContentTree/index.js'

export const View = (props) => (
  <div className='section'>
    <div className='container'>
      <h1 className='title'>Content</h1>
      <div className='columns'>
        <div className='column is-2'>
          <ContentTree
            {...props} />
        </div>
        <div className='column'>
          {props.children}
        </div>
      </div>
    </div>
  </div>
)

View.propTypes = {
  children: React.PropTypes.node,
  describeTree: React.PropTypes.object.isRequired,
  describeTreeRequest: React.PropTypes.func.isRequired,
  createNode: React.PropTypes.object.isRequired,
  createNodeRequest: React.PropTypes.func.isRequired,
  listTypes: React.PropTypes.object.isRequired,
  listTypesRequest: React.PropTypes.func.isRequired,
  deleteNode: React.PropTypes.object.isRequired,
  updateNodeAlias: React.PropTypes.object.isRequired,
  changeType: React.PropTypes.object.isRequired,
  moveNode: React.PropTypes.object.isRequired
}

export default View
