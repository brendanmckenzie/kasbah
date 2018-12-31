import React from 'react'
import PropTypes from 'prop-types'
import ItemButton from '../../../../components/ItemButton'
import AreaComponent from './AreaComponent'

class Area extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    area: PropTypes.string.isRequired,
    onDelete: PropTypes.func.isRequired
  }

  handleAddComponent = () => {
    const { area, input: { value, onChange } } = this.props
    onChange({
      ...value,
      [area]: [
        ...value[area],
        {}
      ]
    })
  }

  render() {
    const { area, input: { value } } = this.props

    return (
      <li>
        <div className='level'>
          <div className='level-left'>
            <code className='subtitle level-item'>{area}</code>
          </div>
          <div className='level-right'>
            <button type='button' className='level-item button is-small is-primary' onClick={this.handleAddComponent}>
              <span>Add component</span>
            </button>
            <ItemButton
              type='button'
              className='level-item button is-small is-warning'
              onClick={this.props.onDelete}
              item={this.props.area}>
              <span className='icon is-small'>
                <i className='fa fa-trash' />
              </span>
            </ItemButton>
          </div>
        </div>

        {value[area].map((ent, index) => <AreaComponent key={index} index={index} ent={ent} {...this.props} />)}
      </li>
    )
  }
}

export default Area
