import React from 'react';
import { Sidebar } from './../sidebar';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import Site from '../../../lib/site';

configure({ adapter: new Adapter() })

function setup(props, isShallow = true) {
    const wrap = isShallow ? (component) => shallow(component) : (component) => mount(component)
    return wrap(<Sidebar {...props} />)
}

describe("Sidebar", () => {
    describe("props", () => {
        it("should call onChange prop with correct url when onChange method is called", () => {
            // Arrange
            const props = {
                onChange: jest.fn()
            }
            const sidebar = new Sidebar(props)
            const fakeData = {
                event: {
                    preventDefault: jest.fn()
                },
                url: 'any'
            }

            // Act
            sidebar.onChange(fakeData.event, fakeData.url)

            // Assert
            expect(props.onChange).toHaveBeenCalledWith(Site.resolveClientUrl(fakeData.url))
        })
    })
})
