import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import _ from 'lodash'
import { actions as contentActions } from 'store/appReducers/content'
import Area from './Area'

class Components extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    content: PropTypes.object.isRequired,
    listComponents: PropTypes.func.isRequired
  }

  static alias = 'kasbah_web:components'

  componentWillMount() {
    if (!this.props.content.components.loaded && !this.props.content.components.loading) {
      this.props.listComponents()
    }
  }

  handleAddArea = () => {
    const { input: { value, onChange } } = this.props
    const area = prompt('Which area?')

    if (area) {
      onChange({
        ...value,
        [area]: []
      })
    }
  }

  handleRemoveArea = (area) => {
    const { input: { value, onChange } } = this.props

    onChange(_.omit(value, area))
  }

  render() {
    const { input: { value, name } } = this.props
    return (
      <div>
        <ul className='field'>
          {_.keys(value).map(area => <Area
            key={area} parent={name} area={area}
            onDelete={this.handleRemoveArea} {...this.props} />)}
        </ul>
        <div className='level'>
          <div className='level-left' />
          <div className='level-right'>
            <button
              type='button'
              className='level-tiem button is-small is-primary'
              onClick={this.handleAddArea}>
              Add area
            </button>
          </div>
        </div>
      </div>
    )
  }
}

const mapStateToProps = (state) => ({
  content: state.content
})

const mapDispatchToProps = {
  ...contentActions
}

export default connect(mapStateToProps, mapDispatchToProps)(Components)
