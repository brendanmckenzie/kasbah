import React from 'react'
import PropTypes from 'prop-types'
import _ from 'lodash'
import Loading from 'components/Loading'
import { makeApiRequest } from 'store/util'
import Area from './Area'

class Components extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired
  }

  static alias = 'kasbah_web:components'

  constructor() {
    super()

    this.state = { loading: true }
  }

  componentWillMount() {
    makeApiRequest({
      url: '/content/components',
      method: 'GET'
    })
      .then(components => {
        this.setState({
          loading: false,
          components
        })
      })
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
    if (this.state.loading) {
      return <Loading />
    }

    const { input: { value, name } } = this.props
    return (
      <div>
        <ul>
          {_.keys(value).map(area => <Area
            key={area} parent={name} area={area}
            onDelete={this.handleRemoveArea} {...this.state} {...this.props} />)}
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

export default Components
