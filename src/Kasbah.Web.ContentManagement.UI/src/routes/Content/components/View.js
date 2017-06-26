import React from 'react'
import PropTypes from 'prop-types'
import ContentTree from './ContentTree/index.js'

class View extends React.Component {
  static propTypes = {
    children: PropTypes.node,
    listTypes: PropTypes.object.isRequired,
    listTypesRequest: PropTypes.func.isRequired
  }

  static childContextTypes = {
    listTypes: PropTypes.object.isRequired
  }

  getChildContext() {
    return {
      listTypes: this.props.listTypes
    }
  }

  componentWillMount() {
    this.props.listTypesRequest()
  }

  render() {
    return (
      <div className='section'>
        <div className='container'>
          <div className='columns'>
            <div className='column is-2'>
              <ContentTree
                {...this.props} />
            </div>
            <div className='column'>
              {this.props.children}
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default View
